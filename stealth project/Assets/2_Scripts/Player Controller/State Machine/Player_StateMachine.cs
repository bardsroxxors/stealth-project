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

    // ####### Component References ########
    private PlayerJumpManager jumpManager;
    private EntityMove_Player em;
    private DashAttack_Player_State dash;
    private Animator animator;
    private Freemove_Player_State freemove;
    private UI_itemBar itembar;
    // #######       -------        ########

    [Header("Grace Timers")]
    public float gracetimePostCollide = 0.2f;
    public float gracetimePreCollide = 0.2f;
    private float t_gracetimePostCollide = 0f;
    private float t_gracetimePreCollide = 0f;
    public float wallJumpNoGrabTime = 0.5f;
    private float t_wallJumpNoGrabTime = 0f;

    [HideInInspector]
    public Utilities utils;

    public SO_AnimationRegister animRegister;
    public Material material;




    private void Start()
    {
        ChangeStateEnum(e_PlayerControllerStates.FreeMove);
        jumpManager = GetComponent<PlayerJumpManager>();
        em = GetComponent<EntityMove_Player>();
        animator = em.graphicsObject.GetComponent<Animator>();
        dash = GetComponent<DashAttack_Player_State>();
        freemove = GetComponent<Freemove_Player_State>();
        utils = new Utilities();

        itembar = GameObject.Find("Equipment panel").GetComponent<UI_itemBar>();
        for (int i = 0; i < equipList.Length; i++)
        {
            itembar.SetIcon(i, equipList[i]);
        }
    }

    private void FixedUpdate()
    {
        if (currentState != null)
            currentState.OnUpdate();

        ManageTimers();
    }

    private void LateUpdate()
    {
        if (playerFacingVector.x != 0)
        {
            em.graphicsObject.transform.localScale = new Vector3(Mathf.Sign(playerFacingVector.x), 1, 1);

            if (playerFacingVector.x < 0)
            {
                if(material != null)
                    material.SetFloat("_Facing_Right", 0);
            }
            else if (playerFacingVector.x > 0 && e_currentState != e_PlayerControllerStates.WallGrab)
            {
                if (material != null)
                    material.SetFloat("_Facing_Right", 1);
            }

        }

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
            case e_PlayerControllerStates.DashAttack:
                s = GetComponent<DashAttack_Player_State>();
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
        if (t_iTime > 0) t_iTime -= Time.deltaTime;
        if (dash.t_attackCooldown > 0) dash.t_attackCooldown -= Time.deltaTime;
        if (t_gracetimePostCollide > 0) t_gracetimePostCollide -= Time.deltaTime;
        if (t_gracetimePreCollide > 0) t_gracetimePreCollide -= Time.deltaTime;
        if (t_wallJumpNoGrabTime > 0) t_wallJumpNoGrabTime -= Time.deltaTime;
    }

    public Vector2 GetVectorToMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - (Vector2)transform.position;
        return direction.normalized;
    }

    public void PlayAnimation(string name, float frame)
    {
        AnimationClip anim = animRegister.GetAnimation(name);

        if (anim != null)
        {
            animator.Play(anim.name, 0, frame);
            //t_currentAnimTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }

    }

    public void PlayAnimation(string name, bool wait, bool overide) // wait means lock any other animations until ths one has finished
    {
        AnimationClip anim = animRegister.GetAnimation(name);

        if (anim != null && (overide || t_currentAnimTime <= 0))
        {
            animator.Play(anim.name, 0);
            if (wait)
                t_currentAnimTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }

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

    void OnLeftMouse(InputValue value)
    {
        //if (currentTarget != null && CheckTargetLOS(currentTarget) && KOTargetValid())
        //TriggerStealthKill();
        //else
        //{
        if (    dash.t_attackCooldown <= 0 &&
                e_currentState == e_PlayerControllerStates.FreeMove &&
                !f_carryingObject &&
                !aiming)

            //ChangeState(e_PlayerControllerStates.SwordSwing);
            ChangeStateEnum(e_PlayerControllerStates.DashAttack);

    }

    void OnToggleSneak(InputValue value)
    {
        //if(CurrentPlayerState == e_PlayerControllerStates.FreeMove)
        //{
        if ( freemove.f_holdToRun) sneaking = false;
        else sneaking = !sneaking;
        //}

    }

    void OnSneakRelease(InputValue value)
    {
        if (freemove.f_holdToRun)
        {
            sneaking = true;

        }
    }

    void OnEquipUp(InputValue value)
    {
        activeEquipIndex -= 1;
        if (activeEquipIndex < 0)
            activeEquipIndex = equipList.Length - 1;
        itembar.SetIndicator(activeEquipIndex);
    }

    void OnEquipDown(InputValue value)
    {
        activeEquipIndex += 1;
        if (activeEquipIndex > equipList.Length - 1)
            activeEquipIndex = 0;
        itembar.SetIndicator(activeEquipIndex);
    }


}
