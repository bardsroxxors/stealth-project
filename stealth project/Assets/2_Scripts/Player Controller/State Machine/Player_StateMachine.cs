using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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


[RequireComponent(typeof(Freemove_Player_State))]
[RequireComponent(typeof(Wallgrab_Player_State))]
[RequireComponent(typeof(Platformgrab_Player_State))]
[RequireComponent(typeof(PlayerJumpManager))]
[RequireComponent(typeof(EntityMove_Player))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PlayerInput))]
public class Player_StateMachine : EntityStateMachine
{
    public e_PlayerControllerStates e_currentState;

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
    public int grabbedDirection = 0;
    [HideInInspector]
    public bool f_groundClose = false;

    [HideInInspector]
    public bool f_isCharging = false;
    [HideInInspector]
    public GameObject grabTarget;
    [HideInInspector]
    public bool grabbedRope = false;
    [HideInInspector]
    public bool f_carryingObject = false;
    [HideInInspector]
    public GameObject carriedObject;
    [HideInInspector]
    public string nextAnim = "";
    private float currentAnimTime = 0;
    private float t_currentAnimTime = 0;
    [HideInInspector]
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

    // Component References
    private PlayerJumpManager jumpManager;
    private EntityMove_Player em;

    [Header("Grace Timers")]
    public float gracetimePostCollide = 0.2f;
    public float gracetimePreCollide = 0.2f;
    private float t_gracetimePostCollide = 0f;
    private float t_gracetimePreCollide = 0f;
    public float wallJumpNoGrabTime = 0.5f;
    private float t_wallJumpNoGrabTime = 0f;



    private void Start()
    {
        ChangeStateEnum(e_PlayerControllerStates.FreeMove);
        jumpManager = GetComponent<PlayerJumpManager>();
        em = GetComponent<EntityMove_Player>();
        Debug.Log("huh");
    }

    public void ChangeStateEnum(e_PlayerControllerStates state)
    {
        Player_State s = GetComponentState(state);
        if(s != null)
        {
            if(ChangeState(s))
                e_currentState = state;
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



    void OnMove(InputValue value)
    {
        moveStickVector.x = value.Get<Vector2>().x;
        moveStickVector.y = value.Get<Vector2>().y;

    }

    void OnJump(InputValue value)
    {

        jumpManager.f_jumpKeyDown = true;

        // if we're on the ground or platform grab
        if (em.GetCollisionDirections().y == -1 || t_gracetimePostCollide > 0 || e_currentState == e_PlayerControllerStates.PlatformGrab)
        {
            if (e_currentState == e_PlayerControllerStates.PlatformGrab)
                ChangeStateEnum(e_PlayerControllerStates.FreeMove);
            em.ResetCollisionY();
            jumpManager.Jump();
        }
        // else if in wall grab
        else if (e_currentState == e_PlayerControllerStates.WallGrab)
        {
            ChangeStateEnum(e_PlayerControllerStates.FreeMove);
            t_wallJumpNoGrabTime = wallJumpNoGrabTime;
            if (moveStickVector.y >= 0) jumpManager.WallJump();
            else if (moveStickVector.y < 0) jumpManager.WallJumpDown();
            em.ResetCollisionX();
            playerFacingVector.x *= -1;
        }
        else
        {
            t_gracetimePreCollide = gracetimePreCollide;
        }

    }
}
