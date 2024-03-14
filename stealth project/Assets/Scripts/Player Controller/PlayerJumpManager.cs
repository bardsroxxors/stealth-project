using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpManager : MonoBehaviour
{


    private PlayerController pc;

    [Header("Options")]
    public bool f_reduceGravityAtPeak = false;
    public bool f_stopOnKeyRelease = false;
    public bool f_peakSpeedBoost = false;

    [Header("Jump")]
    public float jumpForce = 5;
    public LayerMask selfLayerMask;
    public float gravityAccel = 1;
    public float jumpPeakGravityScale = 0.5f;
    public float maxFallSpeed = 10;

    [Header("Stop on Release Options")]
    public float releaseVelocityFactor = 0.5f;

    [Header("Reduce Gravity at Peak Options")]
    public float peakGravityFactor = 0.5f;
    public float peakGravitySpeedWindow = 1f;

    [Header("Peak Speed Boost Options")]
    public float peakBoostFactor = 1.2f;
    public float peakBoostSpeedWindow = 0.5f;

    // used while airborne to check if we started a jump from the ground
    public bool f_jumped = false;

    public bool f_jumpKeyDown = false;






    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
    }



    public void Jump()
    {
        pc.gravityVector.y = jumpForce;
        f_jumped = true;
    }

    public void JumpReleased()
    {
        f_jumpKeyDown = false;

        if (f_jumped && f_stopOnKeyRelease)
        {
            pc.gravityVector.y = pc.gravityVector.y * releaseVelocityFactor;
        }
    }


    public void ApplyGravity()
    {
        // if we're using peak speed boost
        if(f_peakSpeedBoost && Mathf.Abs(pc.gravityVector.y) <= peakBoostSpeedWindow)
        {
            pc.movementVector.x = pc.movementVector.x * peakBoostFactor;
        }


        // if we need to use peak reduced gravity
        if(f_reduceGravityAtPeak && Mathf.Abs(pc.gravityVector.y) <= peakGravitySpeedWindow)
        {
            pc.gravityVector.y -= (gravityAccel * peakGravityFactor) * Time.deltaTime;
        }
        else
        {
            pc.gravityVector.y -= gravityAccel * Time.deltaTime;
        }

        

        // keep fall speed below the max
        if (pc.gravityVector.y < -maxFallSpeed) pc.gravityVector.y = -maxFallSpeed;
    }







    // Input Listener methods
    void OnJumpRelease(InputValue value)
    {
        Debug.Log("jump release");
        JumpReleased();
    }
}
