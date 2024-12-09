using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpManager : MonoBehaviour
{


    private Player_StateMachine psm;

    /*
    [Header("Options")]
    public bool f_reduceGravityAtPeak = false;
    public bool f_stopOnKeyRelease = false;
    public bool f_peakSpeedBoost = false;
    */
    [Header("Jump")]
    public float jumpForce = 5;
    //public LayerMask selfLayerMask;
    //public float gravityAccel = 1;
    //public float jumpPeakGravityScale = 0.5f;
    //public float maxFallSpeed = 10;
    public Vector2 slideJumpFactor = Vector2.one;

    [Header("Wall Jump")]
    public Vector2 wallJumpForceVector = Vector2.zero;
    public Vector2 downJumpVector = Vector2.zero;

    [Header("Stop on Release Options")]
    public float releaseVelocityFactor = 0.5f;

    //[Header("Reduce Gravity at Peak Options")]
    //public float peakGravityFactor = 0.5f;
    //public float peakGravitySpeedWindow = 1f;

    //[Header("Peak Speed Boost Options")]
    //public float peakBoostFactor = 1.2f;
    //public float peakBoostSpeedWindow = 0.5f;

    // used while airborne to check if we started a jump from the ground
    public bool f_jumped = false;

    public bool f_jumpKeyDown = false;
    public bool f_wallGrabReady = false;

    private EntityMovement em;
    private Freemove_Player_State st_free;



    // Start is called before the first frame update
    void Start()
    {
        psm = GetComponent<Player_StateMachine>();
        em = GetComponent<EntityMovement>();
        st_free = GetComponent<Freemove_Player_State>();
    }



    public void Jump()
    {
        if(psm.e_currentState == e_PlayerControllerStates.FreeMove)
            em.SetGravityY( jumpForce);

        if (psm.sliding)
        {
            em.SetGravityX(st_free.moveSpeed * psm.playerFacingVector.x * st_free.slideSpeedFactor * slideJumpFactor.x);
            em.SetGravityY(jumpForce * slideJumpFactor.y);
        }
            

        f_jumped = true;
        f_wallGrabReady = false;
    }

    public void WallJump()
    {
        em.SetGravityX(wallJumpForceVector.x * (psm.grabbedDirection * -1));
        //Debug.Log(wallJumpForceVector.x * (pc.collisionDirections.x * -1));
        em.SetGravityY(wallJumpForceVector.y);
        //pc.playerFacingVector = new Vector2(wallJumpForceVector.x, 0);


        f_jumped = true;
    }

    public void WallJumpDown()
    {
        em.SetGravityX(downJumpVector.x * (psm.grabbedDirection * -1));
        //Debug.Log(wallJumpForceVector.x * (pc.collisionDirections.x * -1));
        em.SetGravityY(downJumpVector.y);
        //pc.playerFacingVector = new Vector2(wallJumpForceVector.x, 0);


        f_jumped = true;
    }

    public void JumpReleased()
    {
        f_jumpKeyDown = false;
        f_wallGrabReady = true;

        if (f_jumped)
        {
            em.SetGravityY( em.GetGravityVector().y * releaseVelocityFactor);
            em.SetMovementY( em.GetGravityVector().y );
        }

        //Debug.Break();
    }


    /*
    public void CalculateGravity()
    {

        if(pc.CurrentPlayerState != e_PlayerControllerStates.WallGrab)
        {
            // if we're using peak speed boost
            // then apply it
            if (f_peakSpeedBoost && Mathf.Abs(em.GetGravityVector().y) <= peakBoostSpeedWindow)
            {
                em.SetMovementX(em.GetMovementVector().x * peakBoostFactor);
            }


            // if we need to use peak reduced gravity
            // then apply it
            if (f_reduceGravityAtPeak && Mathf.Abs(pc.gravityVector.y) <= peakGravitySpeedWindow)
            {
                pc.gravityVector.y -= (gravityAccel * peakGravityFactor) * Time.deltaTime;
            }
            else
            {
                pc.gravityVector.y -= gravityAccel * Time.deltaTime;
            }



            // keep fall speed below the max
            if (pc.gravityVector.y < -maxFallSpeed) pc.gravityVector.y = -maxFallSpeed;


            // decay x component as well
            pc.gravityVector.x = pc.gravityVector.x - (pc.gravityVector.x * (pc.moveDecay/2) * Time.deltaTime);

            // clamp x to zero when its close
            if (Mathf.Abs(pc.gravityVector.x) <= 0.1) pc.gravityVector.x = 0;
        }
        else if (pc.CurrentPlayerState == e_PlayerControllerStates.WallGrab)
        {
            pc.gravityVector.y = 0;
        }
        
    }*/







    // Input Listener methods
    void OnJumpRelease(InputValue value)
    {
        JumpReleased();
    }
}
