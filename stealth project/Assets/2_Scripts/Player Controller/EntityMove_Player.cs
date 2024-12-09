using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityMove_Player : EntityMovement
{
    private Player_StateMachine psm;
    
    [Header("Options")]
    public bool f_reduceGravityAtPeak = false;
    public bool f_stopOnKeyRelease = false;
    public bool f_peakSpeedBoost = false;
    
    /*
    [Header("Jump")]
    public LayerMask selfLayerMask;
    public float jumpPeakGravityScale = 0.5f;
    public Vector2 slideJumpFactor = Vector2.one;*/

    //[Header("Wall Jump")]
    //public Vector2 wallJumpForceVector = Vector2.zero;
    //public Vector2 downJumpVector = Vector2.zero;

    [Header("Stop on Release Options")]
    public float releaseVelocityFactor = 0.5f;

    [Header("Reduce Gravity at Peak Options")]
    public float peakGravityFactor = 0.5f;
    public float peakGravitySpeedWindow = 1f;

    [Header("Peak Speed Boost Options")]
    public float peakBoostFactor = 1.2f;
    public float peakBoostSpeedWindow = 0.5f;


    public override void OnStart()
    {
        base.OnStart();
        psm = GetComponent<Player_StateMachine>();
    }

    public override void CalculateGravity()
    {
        if (psm.e_currentState != e_PlayerControllerStates.WallGrab)
        {
            // if we're using peak speed boost
            // then apply it
            if (f_peakSpeedBoost && Mathf.Abs(GetGravityVector().y) <= peakBoostSpeedWindow)
            {
                SetMovementX(GetMovementVector().x * peakBoostFactor);
            }


            // if we need to use peak reduced gravity
            // then apply it
            if (f_reduceGravityAtPeak && Mathf.Abs(GetGravityVector().y) <= peakGravitySpeedWindow)
            {
                SetGravityY( GetGravityVector().y - (gravityAccel * peakGravityFactor) * Time.deltaTime);
            }
            else
            {
                SetGravityY( GetGravityVector().y - gravityAccel * Time.deltaTime);
            }



            // keep fall speed below the max
            if (GetGravityVector().y < -maxFallSpeed) SetGravityY(-maxFallSpeed);


            // decay x component as well
            SetGravityX( GetGravityVector().x  -  (GetGravityVector().x * (moveDecay / 2) * Time.deltaTime) );

            // clamp x to zero when its close
            if (Mathf.Abs(GetGravityVector().x) <= 0.1) SetGravityX(0);
        }
        else if (       psm.e_currentState == e_PlayerControllerStates.WallGrab 
                    ||  psm.e_currentState == e_PlayerControllerStates.PlatformGrab)
        {
            Debug.Log("no grabity");
            SetGravityY(0);
        }
    }

    
}
