using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallgrab_Player_State : Player_State
{

    [Header("Wall Grabbing")]
    public bool canWallGrab = true;
    public Vector2 wallJumpForce = Vector2.zero;
    public int grabbedDirection = 0;
    public float climbSpeed = 0.5f;
    private bool f_initialRopePos = false; // used to check if we've done the initial "stick to rope"
    private bool f_zippedToRope = false;

    public float wallJumpNoGrabTime = 0.5f;
    [HideInInspector]
    public float t_wallJumpNoGrabTime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
