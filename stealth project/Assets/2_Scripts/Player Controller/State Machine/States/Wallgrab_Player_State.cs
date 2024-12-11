using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallgrab_Player_State : Player_State
{

    [Header("Wall Grabbing")]
    public bool canWallGrab = true;
    //public Vector2 wallJumpForce = Vector2.zero;
    public float climbSpeed = 0.5f;
    private bool f_initialRopePos = false; // used to check if we've done the initial "stick to rope"
    private bool f_zippedToRope = false;

    public float wallJumpNoGrabTime = 0.5f;
    [HideInInspector]
    public float t_wallJumpNoGrabTime = 0f;


    // Start is called before the first frame update
    public override void OnStart()
    {
        base.OnStart();
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        em.inputVector.x = 0;
        // set player facing based on collision direction
        if (em.GetCollisionDirections().x != 0)
        {
            psm.playerFacingVector = new Vector2(em.GetCollisionDirections().x, 0);
            psm.grabbedDirection = (int)Mathf.Sign(em.GetCollisionDirections().x);
        }



        Vector3 center = collider.bounds.center;
        float height = collider.bounds.size.y / 2;

        Vector3 leftPoint = center + new Vector3(-collider.bounds.size.x / 2, 0, 0);
        Vector3 rightPoint = center + new Vector3(collider.bounds.size.x / 2, 0, 0);


        float range = 0.05f;
        Vector2 vSize = new Vector2(range * 2, height);

        RaycastHit2D box = new RaycastHit2D();

        if (em.GetCollisionDirections().x == -1)
            box = Physics2D.BoxCast(leftPoint, vSize, 0, Vector2.left, 0, psm.collisionMask);
        else if (em.GetCollisionDirections().x == 1)
            box = Physics2D.BoxCast(rightPoint, vSize, 0, Vector2.right, 0, psm.collisionMask);




        if (em.GetCollisionDirections().y == -1)
        {
            psm.playerFacingVector.x = psm.playerFacingVector.x * -1;
            psm.ChangeStateEnum(e_PlayerControllerStates.FreeMove);
        }



        // get inputVector from raw input, set player facing
        if (psm.moveStickVector.magnitude >= 0.25)
        {
            em.inputVector.y = psm.moveStickVector.normalized.y * climbSpeed;
        }


        if (!box && em.inputVector.y > 0)
        {
            // get grid-snapped position, suedo grid position
            // place at y+1, x+-1

            float gridDistance = 1f / 2f;

            Vector3 snap = new Vector3(transform.position.x - ((transform.position.x % gridDistance) * psm.grabbedDirection),
                                        transform.position.y - (transform.position.y % gridDistance),
                                        transform.position.z);

            snap.x += psm.grabbedDirection * 0.5f;
            snap.y += 0.5f;

            transform.position = snap;
            psm.ChangeStateEnum(e_PlayerControllerStates.FreeMove);
        }
        else if (!box)
        {
            psm.playerFacingVector.x *= -1;
            psm.ChangeStateEnum(e_PlayerControllerStates.FreeMove);
        }
    }
}
