using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum e_PlayerControllerStates
{
    FreeMove,
    SwordSwing,
    DashAttack,
    WallGrab,
    PlatformGrab,
    Hiding,
    Hurt,
    StealthKill,
    Blink,
    Dead
}


public class Player_StateMachine : EntityStateMachine
{

    public bool lit = false;
    public bool sneaking = false;
    public bool crouching = false;
    public bool aiming = false;
    public bool sliding = false;
    public int currentHP = 10;
    public int maxHP = 10;
    public bool f_insideEnemy = false;
    private GameObject shovingEnemy;
    public float shoveForce = 2f;
    private float t_iTime = 0;

    [HideInInspector]
    public bool f_groundClose = false;

    [HideInInspector]
    public bool f_isCharging = false;

    public GameObject grabTarget;
    [HideInInspector]
    public bool grabbedRope = false;
    public bool f_carryingObject = false;
    public GameObject carriedObject;

    public string nextAnim = "";
    private float currentAnimTime = 0;
    private float t_currentAnimTime = 0;

    public int activeEquipIndex = 0;
    public e_Equipment[] equipList = new e_Equipment[4];

    public LayerMask collisionMask;


    [HideInInspector]
    public Vector2 moveStickVector = new Vector2(0, 0); // raw input from left stick / wasd
    [HideInInspector]
    public Vector2 aimStickVector = new Vector2(0, 0); // raw input from right stick
    [HideInInspector]
    public Vector2 playerFacingVector = new Vector2(1, 0); // used to aim abilities if there is no input, and used to determine sprite facing

    public float airFrameSpeedThreshold = 3;
    

    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ChangeStateEnum(e_PlayerControllerStates state)
    {
        Player_State s = GetComponentState(state);
        if(s != null)
        {
            ChangeState(s);
        }
    }

    public Player_State GetComponentState(e_PlayerControllerStates state)
    {
        Player_State s = null;
        switch (state)
        {
            case e_PlayerControllerStates.FreeMove:
                s = GetComponent<Freemove_Player_State>(); 
                break;
            case e_PlayerControllerStates.WallGrab:
                s = GetComponent<Wallgrab_Player_State>();
                break;
            case e_PlayerControllerStates.PlatformGrab:
                s = GetComponent<Platformgrab_Player_State>();
                break;
        }

        return s;
    }

    public void SetITime(float t) 
    {
        t_iTime = t;
    }

    private void ManageTimers()
    {
        // t_iTime
    }

    public Vector2 GetVectorToMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - (Vector2)transform.position;
        return direction.normalized;
    }
}
