using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Freemove_Player_State : Player_State
{


    [Header("Free Move")]
    public bool f_holdToRun = true;
    
    public float moveSpeed = 1;
    public float sneakSpeed = 1;
    public float crouchSpeed = 1;
    public float moveDecay = 10;

    public GameObject noisePrefab;
    public bool f_noiseAnimationTrigger = false;
    public Sound footstepSound;

    [Header("Sliding")]
    public float slideDecelFactor = 5;
    public float slideSpeedFactor = 1.2f;
    public float slideCooldown = 1;
    private float t_slideCooldown = 0;
    private int slideDirection = 0;
    private bool crouchReleased = true;
    private float slideSpeedLastFrame = 0;
    public float slideITime = 0.25f;

    private Wallgrab_Player_State wallg;
    private PlayerJumpManager jumpMan;


    // Start is called before the first frame update
    public override void OnStart()
    {
        base.OnStart();
        wallg = GetComponent<Wallgrab_Player_State>();
        jumpMan = GetComponent<PlayerJumpManager>();
    }

    public override void OnUpdate()
    {
        CheckCrouching();
        

        if (psm.aiming)
        {
            psm.playerFacingVector.x = Mathf.Sign(psm.GetVectorToMouse().x);
        }


        CheckSlideStop();

        CheckSlideStart();

        if (psm.crouching) crouchReleased = false;

        ApplyInput();

        CheckWallGrab();
        
        CheckPlatformGrab();
        
        if(em.GetMovementVector().magnitude > 1)
        {
            psm.PlayAnimation("walk", false, false);
        }
        else
        {
            psm.PlayAnimation("idle", false, false);
        }

        // create noise when triggered
        if (f_noiseAnimationTrigger)
        {
            f_noiseAnimationTrigger = false;
            GameObject noise = Instantiate(noisePrefab, transform.position, Quaternion.identity);
            noise.SendMessage("SetProfile", footstepSound);
        }


    }

    private void CheckCrouching()
    {
        // check if we are crouching
        if (psm.moveStickVector.y < 0 && psm.f_groundClose) psm.crouching = true;
        else psm.crouching = false;

        if (!psm.crouching && !crouchReleased)
        {
            crouchReleased = true;

        }
    }

    private void CheckSlideStart()
    {
        // check if we started sliding
        if (psm.crouching && !psm.sliding && crouchReleased && t_slideCooldown <= 0/* && !sneaking*/)
        {
            psm.sliding = true;
            psm.SetITime(slideITime);
            crouchReleased = false;
            slideDirection = Math.Sign(psm.moveStickVector.x);
            slideSpeedLastFrame = Mathf.Abs(psm.moveStickVector.normalized.x * moveSpeed * slideSpeedFactor);
        }
    }

    private void CheckSlideStop()
    {
        // we stop sliding if we change direction or jump or speed reaches zero (or change state do that elseswhere)
        if (Math.Sign(psm.moveStickVector.x) != slideDirection ||
            psm.moveStickVector.x == 0 ||
            em.GetCollisionDirections().y != -1 ||
            !psm.crouching)
        {
            if (psm.sliding)
                t_slideCooldown = slideCooldown;
            psm.sliding = false;

        }
    }

    private void ApplyInput()
    {
        // get inputVector from raw input, set player facing
        if (psm.moveStickVector.magnitude >= 0.25 && !(psm.aiming && em.GetCollisionDirections().y == -1))
        {
            if (!psm.sneaking && !psm.crouching && !psm.f_isCharging && !psm.sliding)
                em.inputVector.x = psm.moveStickVector.normalized.x * moveSpeed;
            else if (psm.sliding)
            {
                // movestick vector is between 0 and 1, so its necessaesry to keep sliding
                // then I want to take the previous speed and reduce it
                float speed = slideSpeedLastFrame - (Time.deltaTime * slideDecelFactor * slideSpeedLastFrame);
                if (speed < 0.5f)
                {
                    psm.sliding = false;
                    speed = 0;
                }
                em.inputVector.x = speed * slideDirection;
                slideSpeedLastFrame = Mathf.Abs(em.inputVector.x);
            }
            else if (psm.crouching)
                em.inputVector.x = psm.moveStickVector.normalized.x * crouchSpeed;
            else em.inputVector.x = psm.moveStickVector.normalized.x * sneakSpeed;
            em.inputVector.y = 0;
            psm.playerFacingVector = psm.moveStickVector.normalized;
        }
    }

    private void CheckWallGrab()
    {
        // change to wall grab under right conditions
        if (em.GetCollisionDirections().x != 0)
        {

            Vector3 center = collider.bounds.center;
            float height = collider.bounds.size.y/2;

            Vector3 leftPoint = center + new Vector3(-collider.bounds.size.x / 2, 0, 0);
            Vector3 rightPoint = center + new Vector3(collider.bounds.size.x / 2, 0, 0);


            float range = 0.05f;
            Vector2 vSize = new Vector2(range * 2, height);

            RaycastHit2D box = new RaycastHit2D();

            if(em.GetCollisionDirections().x == -1)
                box = Physics2D.BoxCast(leftPoint, vSize, 0, Vector2.left, 0, psm.collisionMask);
            else if (em.GetCollisionDirections().x == 1)
                box = Physics2D.BoxCast(rightPoint, vSize, 0, Vector2.right, 0, psm.collisionMask);



            /*
            if (box)
            {

                Debug.DrawLine(transform.position + new Vector3(collider.bounds.max.x * em.GetCollisionDirections().x, 0, 0),
                            transform.position + new Vector3(em.GetCollisionDirections().x, 0), Color.cyan);
            }*/
                

            

            if (box &&
                em.GetCollisionDirections().y != -1 &&
                wallg.t_wallJumpNoGrabTime <= 0 &&
                jumpMan.f_jumpKeyDown &&
                jumpMan.f_wallGrabReady)
            {
                psm.ChangeStateEnum(e_PlayerControllerStates.WallGrab);
            }
        }
    }

    private void CheckPlatformGrab()
    {
        // change to platform grab under right conditions
        if (psm.grabTarget != null &&
                em.GetCollisionDirections().y != -1 &&
                wallg.t_wallJumpNoGrabTime <= 0 &&
                jumpMan.f_jumpKeyDown &&
                jumpMan.f_wallGrabReady)
        {
            if (psm.grabTarget.tag == "Rope")
                psm.grabbedRope = true;
            else
                psm.grabbedRope = false;
            psm.ChangeStateEnum(e_PlayerControllerStates.PlatformGrab);
        }
    }

}