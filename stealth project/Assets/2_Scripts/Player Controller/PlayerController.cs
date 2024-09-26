using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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

public enum e_Equipment
{
    empty,
    sword,
    bearTrap,
    arrow,
    blink,
    kunai,
    forceMine
}

public enum e_ControlSchemes
{
    MouseKeyboard,
    Gamepad
}

public class PlayerController : MonoBehaviour
{

    public Material material;

    [SerializeField]
    public e_PlayerControllerStates CurrentPlayerState = e_PlayerControllerStates.FreeMove;
    private e_PlayerControllerStates previousPlayerState = e_PlayerControllerStates.FreeMove;

    [SerializeField]
    private e_ControlSchemes controlScheme = e_ControlSchemes.MouseKeyboard;

    //private PlayerAttackManager playerAttacks;

    public bool lit = false;
    public bool sneaking = false;
    public bool crouching = false;
    public bool sliding = false;
    public int currentHP = 10;
    public int maxHP = 10;
    public bool f_insideEnemy = false;
    private GameObject shovingEnemy;
    public float shoveForce = 2f;
    private float t_iTime = 0;

    public int activeEquipIndex = 0;
    public e_Equipment[] equipList = new e_Equipment[4]; 

    [Header("Free Move")]
    public bool f_holdToRun = true;
    public bool f_carryingObject = false;
    public GameObject carriedObject;
    public float moveSpeed = 1;
    public float sneakSpeed = 1;
    public float crouchSpeed = 1;
    //public float acceleration = 5;
    public float moveDecay = 10;
    public Vector2 inputVector = new Vector2(0, 0); // the vector being used as a handle from the user input to the movement vector
    public Vector2 movementVector = new Vector2(0, 0); // vector that is used to store the desired movement, before any checks
    private Vector2 moveStickVector = new Vector2(0, 0); // raw input from left stick / wasd
    private Vector2 aimStickVector = new Vector2(0, 0); // raw input from right stick
    public Vector2 playerFacingVector = new Vector2(1, 0); // used to aim abilities if there is no input, and used to determine sprite facing
    public Vector2 gravityVector = Vector2.zero;

    [Header("Wall Grabbing")]
    public bool canWallGrab = true;
    public Vector2 wallJumpForce = Vector2.zero;
    public int grabbedDirection = 0;
    public float climbSpeed = 0.5f;

    [Header("Sliding")]
    public float slideDecelFactor = 5;
    public float slideSpeedFactor = 1.2f;
    public float slideCooldown = 1;
    private float t_slideCooldown = 0;
    private int slideDirection = 0;
    private bool crouchReleased = true;
    private float slideSpeedLastFrame = 0;
    public float slideITime = 0.25f;

    [Header("Dash Attack")]
    public bool f_canDashAttack = true;
    public float dashLockTime = 0.5f;
    public float dashSpeed = 10f;
    public float dashAttackRange = 1f;
    private float t_dashLockTime = 0;

    [Header("Hiding")]
    //public GameObject hidingPlace;

    public GameObject interactTarget;
    public GameObject hideTarget;

    [Header("Platform Grab")]
    public float minGrabDistance = 0.05f;
    public GameObject grabTarget;

    [Header("Sword Swing")]
    public GameObject swordObject;
    public float attackCooldown = 0.5f;
    private float t_attackCooldown = 0;
    private bool f_init_swordSwing = false;
    public float swingMoveSpeed = 5f;
    public float swingMoveDecay = 5f;
    [Range(0, 1)]
    public float attackLength = 0.5f;
    
    private float t_attackLength = 0;

    private bool f_canInteract = false;

    [Header("Stealth Kills")]
    private bool f_init_stealthKill = false;
    public float killChargeTime = 1f;
    private float t_killChargeTime = 0f;
    public float chargingMoveSpeed = 3f;
    public bool f_isCharging = false;
    public GameObject killzoneObject;
    private StealthKillZone killZone;
    public float minZipDistance = 0.2f;
    [Range(0,1)]
    public float zipSpeed = 2f;
    public GameObject currentTarget;
    public GameObject koIndicator;
    private Vector3 koStartPos = Vector3.zero;
    private bool f_kozip = false;

    [Header("Collisions")]
    public Vector2 collisionDirections = Vector2.zero; // set to 0 for no collision, -1 for left, 1 for right
    public string[] collisionLayers;
    public Vector2 groundNormal = Vector2.zero;
    public float colliderYModifier = 0.6f;
    public float colliderYOffset = -0.05f;
    private float colliderYscale = 0;

    public GameObject normalIndicator;
    public bool slopeCheckRaycast = false;
    public LayerMask slopeMask;
    public LayerMask collisionMask;
    public LayerMask lightCheckMask;
    public LayerMask koCheckMask;
    public float slopeRaycastDistance = 1;
    public Sound footstepSound;
    public Sound meowSound;

    public UI_Backpack backpack;



    //private BoxCollider2D collider;
    //public GameObject colliderObject;
    private PlayerJumpManager jumpManager;
    private Animator animator;
    public GameObject graphicsObject;
    private GameObject ScoreManager;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D collider;
    private Color defaultColour;
    public Color darkColour;
    public GameObject noisePrefab;
    public float noiseInterval = 1;
    private float t_noiseInterval = 0;
    public bool f_noiseAnimationTrigger = false;

    [Header("Grace Timers")]
    public float gracetimePostCollide = 0.2f;
    public float gracetimePreCollide = 0.2f;
    private float t_gracetimePostCollide = 0f;
    private float t_gracetimePreCollide = 0f;

    public float wallJumpNoGrabTime = 0.5f;
    private float t_wallJumpNoGrabTime = 0f;

    private bool f_groundClose = false;

    [Header("Knockback")]
    public float knockTime = 0.3f;
    private float t_knockTime = 0;
    public Vector2 knockVector = Vector2.zero;
    private int knockDirection = 0;

    public GameObject contextButton;
    public GameObject paperSword;

    [Header("Blink Power")]
    public bool f_blinkAiming = false;
    public float blinkRange = 3f;
    private bool f_init_blink = false;
    private Vector3 blinkPosition = Vector3.zero;
    public float blinkSpeed = 8f;
    public float blinkCooldown = 2f;
    private float t_blinkCooldown = 0f;
    public GameObject blinkAimObject;
    public float blinkAirBop = 5f;
    private Vector2 tempMoveV = Vector2.zero;
    private Vector2 tempGravityV = Vector2.zero;

    
    [Header("Equipment References")]
    public GameObject baseProjectile;
    /*
    public Projectile so_bearTrap;
    public Projectile so_arrow;
    public Projectile so_kunai;
    public Projectile so_forceMine;*/

    Dictionary<e_Equipment, Projectile> dict_projectiles_enumSO = new Dictionary<e_Equipment, Projectile>();
    public SO_EquipRegister EquipmentRegister;

    private Utilities utils = new Utilities();
    private UI_itemBar itembar;




    void Start()
    {
        //collider = colliderObject.GetComponent<BoxCollider2D>();
        jumpManager = GetComponent<PlayerJumpManager>();
        spriteRenderer = graphicsObject.GetComponent<SpriteRenderer>();
        defaultColour = spriteRenderer.color;
        animator = graphicsObject.GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        killZone = killzoneObject.GetComponent<StealthKillZone>();
        //backpack = GameObject.Find("Backpack").GetComponent<UI_Backpack>();
        ScoreManager = GameObject.Find("Points Manager");
        colliderYscale = collider.size.y;


        for (int i = 0; i < EquipmentRegister.enums.Count; i++)
        {
            dict_projectiles_enumSO.Add(EquipmentRegister.enums[i], EquipmentRegister.projectiles[i]);
        }

        itembar = GameObject.Find("Equipment panel").GetComponent<UI_itemBar>();

        for (int i = 0; i < equipList.Length; i++)
        {
            itembar.SetIcon(i, equipList[i]);
        }

    }


    private void LateUpdate()
    {
        if(playerFacingVector.x != 0)
        {
            graphicsObject.transform.localScale = new Vector3(Mathf.Sign(playerFacingVector.x), 1, 1);

            if (playerFacingVector.x < 0)
            {
                material.SetFloat("_Facing_Right", 0);
                paperSword.GetComponent<SpriteRenderer>().sortingOrder = -1;
            }
            else if (playerFacingVector.x > 0 && CurrentPlayerState != e_PlayerControllerStates.WallGrab)
            {
                material.SetFloat("_Facing_Right", 1);
                paperSword.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }

        }
            
    }

    void FixedUpdate()
    {
        if (controlScheme == e_ControlSchemes.MouseKeyboard) aimStickVector = GetVectorToMouse();


        CheckColliderSize();

        

        // Process the current state
        switch (CurrentPlayerState)
        {
            case e_PlayerControllerStates.FreeMove:
                ProcessFreeMove();
                break;
            case e_PlayerControllerStates.WallGrab:
                ProcessWallGrab();
                break;
            case e_PlayerControllerStates.SwordSwing:
                ProcessSwordSwing();
                break;
            case e_PlayerControllerStates.DashAttack:
                ProcessDashAttack();
                break;
            case e_PlayerControllerStates.Hurt:
                ProcessHurt();
                break;
            case e_PlayerControllerStates.Hiding:
                ProcessHiding();
                break;
            case e_PlayerControllerStates.StealthKill:
                ProcessStealthKill();
                break;
            case e_PlayerControllerStates.Blink:
                ProcessBlink();
                break;
            case e_PlayerControllerStates.PlatformGrab:
                ProcessPlatformGrab();
                break;
        }


        if(currentTarget != null && KOTargetValid())
        {
            koIndicator.SetActive(true);
            koIndicator.GetComponent<KOIndicator>().targetPosition = currentTarget.transform.position;
        }
        else
            koIndicator.SetActive(false);


        // carrying object stuff
        if(f_carryingObject && carriedObject != null)
        {
            carriedObject.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        }

        // Blink stuff
        if (equipList[activeEquipIndex] != e_Equipment.blink && CurrentPlayerState == e_PlayerControllerStates.FreeMove ||
                                                                CurrentPlayerState == e_PlayerControllerStates.WallGrab)
            f_blinkAiming = false;
        else if (f_blinkAiming)
        {
            AimBlink();
        }
        else
        {
            blinkAimObject.SetActive(false);
        }




        if (CurrentPlayerState != e_PlayerControllerStates.Hiding && CurrentPlayerState != e_PlayerControllerStates.Blink)
            ApplyMovement();

        if(!(CurrentPlayerState == e_PlayerControllerStates.FreeMove ||
            CurrentPlayerState == e_PlayerControllerStates.WallGrab ) && f_isCharging)
            f_isCharging = false;


        // manage timers
        /*
        if (f_isCharging)
        {
            t_killChargeTime += Time.deltaTime;
            float percent = t_killChargeTime / killChargeTime;
            if (percent > 1) percent = 1;
            koIndicator.GetComponent<KOIndicator>().animationPercent = percent;

            //currentTarget = GetKillTarget();
            if (currentTarget != null && CheckTargetLOS(currentTarget))
            {
                koIndicator.transform.position = currentTarget.transform.position;
            }
            else koIndicator.transform.localPosition = new Vector3(0,1,0);

        }
        else
        {
            t_killChargeTime = 0;
            koIndicator.SetActive(false);
        }*/

        // manage timers
        if (t_gracetimePostCollide > 0) t_gracetimePostCollide -= Time.deltaTime;
        if (t_gracetimePreCollide > 0) t_gracetimePreCollide -= Time.deltaTime;
        if (t_wallJumpNoGrabTime > 0) t_wallJumpNoGrabTime -= Time.deltaTime;
        if (t_attackCooldown > 0) t_attackCooldown -= Time.deltaTime;
        if (t_knockTime > 0) t_knockTime -= Time.deltaTime;
        if (t_noiseInterval > 0) t_noiseInterval -= Time.deltaTime;
        if (t_attackLength > 0) t_attackLength -= Time.deltaTime;
        if (t_slideCooldown > 0) t_slideCooldown -= Time.deltaTime;
        if (t_iTime > 0) t_iTime -= Time.deltaTime;
        if (t_blinkCooldown > 0) t_blinkCooldown -= Time.deltaTime;
        if (t_dashLockTime > 0) t_dashLockTime -= Time.deltaTime;

    }

    private void Update()
    {
        //if (lit) spriteRenderer.color = defaultColour;
        //else spriteRenderer.color = darkColour;
        UpdateAnimator();

        Debug.DrawRay(transform.position, GetVectorToMouse());
        
    }


    private void CheckColliderSize()
    {
        // if we're on the ground
        if (collisionDirections.y == -1 || f_groundClose)
        {
            collider.size = new Vector2(collider.size.x, colliderYscale);
            collider.offset = new Vector2(collider.offset.x, colliderYOffset);
        }
        else
        {
            collider.size = new Vector2(collider.size.x, colliderYscale * colliderYModifier);
            collider.offset = new Vector2(collider.offset.x, colliderYOffset + ((1 - colliderYModifier) / 2));
        }
    }


    // Update Functions for States

    void ProcessFreeMove()
    {

        if (f_canInteract && !contextButton.active) contextButton.SetActive(true);
        else if (!f_canInteract && contextButton.active) contextButton.SetActive(false);

        // check if we are crouching
        if (moveStickVector.y < 0 && f_groundClose) crouching = true;
        else crouching = false;

        if (!crouching && !crouchReleased)
        {
            crouchReleased = true;
            
        }

        

        
        // we stop sliding if we change direction or jump or speed reaches zero (or change state do that elseswhere)
        if(Math.Sign(moveStickVector.x) != slideDirection ||
            moveStickVector.x == 0 ||
            collisionDirections.y != -1 ||
            !crouching)
        {
            if(sliding)
                t_slideCooldown = slideCooldown;
            sliding = false;
            
        }

        // check if we started sliding
        if (crouching && !sliding && crouchReleased && t_slideCooldown <= 0/* && !sneaking*/)
        {
            sliding = true;
            t_iTime = slideITime;
            crouchReleased = false;
            slideDirection = Math.Sign(moveStickVector.x);
            slideSpeedLastFrame = Mathf.Abs(moveStickVector.normalized.x * moveSpeed * slideSpeedFactor);
        }

        if (crouching) crouchReleased = false;



        // get inputVector from raw input, set player facing
        if (moveStickVector.magnitude >= 0.25)
        {
            if(!sneaking && !crouching && !f_isCharging && !sliding/* && collisionDirections.y != -1*/) 
                inputVector.x = moveStickVector.normalized.x * moveSpeed;
            else if (sliding)
            {
                // movestick vector is between 0 and 1, so its necessaesry to keep sliding
                // then I want to take the previous speed and reduce it
                float speed = slideSpeedLastFrame - (Time.deltaTime * slideDecelFactor * slideSpeedLastFrame);
                if (speed < 0.5f)
                {
                    sliding = false;
                    speed = 0;
                }
                inputVector.x = speed * slideDirection;
                slideSpeedLastFrame = Mathf.Abs(inputVector.x);
            }
            else if (crouching) 
                inputVector.x = moveStickVector.normalized.x * crouchSpeed;
            else inputVector.x = moveStickVector.normalized.x * sneakSpeed;
            inputVector.y = 0;
            playerFacingVector = moveStickVector.normalized;
        }

        // if there is no input then apply movement decay
        else if (inputVector.magnitude > 0)
        {
            inputVector.x = inputVector.x - (inputVector.x * moveDecay * Time.deltaTime);
        }

        // clamp to zero when its close
        if (inputVector.magnitude <= 0.1) inputVector = Vector2.zero;


        
        // clamp gravity x to zero when its close
        if (Mathf.Abs(gravityVector.x) <= 0.15) gravityVector.x = 0;


        // change to wall grab under right conditions
        if (collisionDirections.x != 0)
        {
            RaycastHit2D wallCheck = Physics2D.BoxCast(transform.position,
                                                    new Vector2(0.2f, 0.2f),
                                                    0,
                                                    new Vector2(collisionDirections.x * 0.2f, 0),
                                                    1,
                                                    collisionMask);
            if (wallCheck &&
                collisionDirections.y != -1 &&
                t_wallJumpNoGrabTime <= 0 &&
                jumpManager.f_jumpKeyDown &&
                jumpManager.f_wallGrabReady &&
                canWallGrab)
            {
                ChangeState(e_PlayerControllerStates.WallGrab);
            }
        }

        // change to platform grab under right conditions
        if (grabTarget != null &&
                collisionDirections.y != -1 &&
                t_wallJumpNoGrabTime <= 0 &&
                jumpManager.f_jumpKeyDown &&
                jumpManager.f_wallGrabReady &&
                canWallGrab)
        {
            ChangeState(e_PlayerControllerStates.PlatformGrab);
        }


        // create noise when triggered
        if (f_noiseAnimationTrigger)
        {
            f_noiseAnimationTrigger = false;
            GameObject noise = Instantiate(noisePrefab, transform.position, Quaternion.identity);
            noise.SendMessage("SetProfile", footstepSound);
        }
        

    }


    private void ProcessWallGrab()
    {
        

        inputVector.x = 0;
        // set player facing based on collision direction
        if(collisionDirections.x != 0)
        {
            playerFacingVector = new Vector2(collisionDirections.x, 0);
            grabbedDirection = (int)Mathf.Sign(collisionDirections.x);
        }
            


        RaycastHit2D wallCheck = Physics2D.BoxCast(transform.position,
                                                    new Vector2(0.2f, 0.2f),
                                                    0, 
                                                    new Vector2(grabbedDirection*0.2f, 0), 
                                                    1,
                                                    collisionMask);

        


        if (collisionDirections.y == -1)
        {
            playerFacingVector.x = playerFacingVector.x * -1;
            ChangeState(e_PlayerControllerStates.FreeMove);
        }



        // get inputVector from raw input, set player facing
        if (moveStickVector.magnitude >= 0.25)
        {
            inputVector.y = moveStickVector.normalized.y * climbSpeed;
        }

        // if there is no input then apply movement decay
        else if (inputVector.magnitude > 0)
        {
            inputVector.y = inputVector.y - (inputVector.y * moveDecay * Time.deltaTime);
        }

        // clamp to zero when its close
        if (inputVector.magnitude <= 0.1) inputVector = Vector2.zero;

        if (!wallCheck && inputVector.y > 0)
        {
            // get grid-snapped position, suedo grid position
            // place at y+1, x+-1

            float gridDistance = 1f / 2f;

            Vector3 snap = new Vector3(transform.position.x - ((transform.position.x % gridDistance) * grabbedDirection),
                                        transform.position.y - (transform.position.y % gridDistance),
                                        transform.position.z);

            snap.x += grabbedDirection * 0.5f;
            snap.y += 0.5f;

            transform.position = snap;

            ChangeState(e_PlayerControllerStates.FreeMove);
        }
        else if (!wallCheck)
        {
            playerFacingVector.x *= -1;
            ChangeState(e_PlayerControllerStates.FreeMove);
        }


    }


    private void ProcessSwordSwing()
    {
        if (!f_init_swordSwing)
        {
            

            if(collisionDirections.y == -1)
            {
                swordObject.SetActive(true);
                swordObject.transform.GetChild(0).transform.gameObject.SetActive(true);
                swordObject.transform.GetChild(1).transform.gameObject.SetActive(false);

                swordObject.transform.position = transform.position;
                swordObject.transform.localScale = new Vector3(playerFacingVector.x, 1, 1);
                swordObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("swing");
                swordObject.transform.GetChild(0).GetComponent<SwordScript>().animating = true;

                swordObject.transform.GetChild(0).transform.localPosition = new Vector3(1f, 0, 0);
                swordObject.transform.GetChild(0).GetComponent<DamageSource>().RefreshDamageSource();

                //inputVector.x = swingMoveSpeed * playerFacingVector.x;

                animator.Play("attack", 0);
            }
            else // airborn variation
            {
                swordObject.SetActive(true);
                swordObject.transform.GetChild(1).transform.gameObject.SetActive(true);
                swordObject.transform.GetChild(0).transform.gameObject.SetActive(false);

                swordObject.transform.position = transform.position;
                swordObject.transform.localScale = new Vector3(playerFacingVector.x, 1, 1);
                swordObject.transform.GetChild(1).GetComponent<Animator>().SetTrigger("air swing");
                swordObject.transform.GetChild(1).GetComponent<SwordScript>().animating = true;

                swordObject.transform.GetChild(1).transform.localPosition = new Vector3(0.7f, 0, 0);
                swordObject.transform.GetChild(1).GetComponent<DamageSource>().RefreshDamageSource();

            }

            t_attackLength = attackLength;
            t_attackCooldown = attackCooldown;
            //animator.SetTrigger("attack trigger");
            

            f_init_swordSwing = true;
        }

        //if(collisionDirections.y != -1 && moveStickVector.magnitude >= 0.25)
        if (moveStickVector.magnitude >= 0.25)
            inputVector.x = moveStickVector.normalized.x * moveSpeed;

        // clamp to zero when its close
        if (inputVector.magnitude <= 0.1) inputVector = Vector2.zero;

        if (inputVector.magnitude > 0)
        {
            inputVector.x = inputVector.x - (inputVector.x * swingMoveDecay * Time.deltaTime);
        }

        //gravityVector.y = 0;

        if(collisionDirections.y == -1)
        {
            if (!swordObject.transform.GetChild(0).GetComponent<SwordScript>().animating)
                swordObject.SetActive(false);
        }
        else
        {
            if (!swordObject.transform.GetChild(1).GetComponent<SwordScript>().animating)
                swordObject.SetActive(false);
        }

        

        
        if(t_attackLength <= 0)
        {
            swordObject.SetActive(false);
            if(     moveStickVector.magnitude > 0.1f ||
                    animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "attack") 
                        ChangeState(e_PlayerControllerStates.FreeMove);

        }

    }

    private void ProcessDashAttack()
    {
        // ok so to do the dash attack what do we need to do?
        // use the same sword slash object but change its rotation in relation to the mouse
        // gain a burst of speed when you do it
        // have a short period where gravity does not apply and you can't move left right
        // have a boolean that resets when you touch the ground, that way you can use it like a kind of double jump
        // which is pretty awesome

        Vector2 attackDirection = (Vector3)GetVectorToMouse();

        if (!f_init_swordSwing)
        {

            collisionDirections = Vector2.zero;
            playerFacingVector = new Vector2(Mathf.Sign(attackDirection.x), 0);
            
            swordObject.SetActive(true);
            swordObject.transform.GetChild(0).transform.gameObject.SetActive(true);
            swordObject.transform.GetChild(1).transform.gameObject.SetActive(false);

            swordObject.transform.position = transform.position;
            swordObject.transform.localScale = new Vector3(playerFacingVector.x, 1, 1);
            swordObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("swing");
            swordObject.transform.GetChild(0).GetComponent<SwordScript>().animating = true;



            attackDirection.x = Mathf.Abs(attackDirection.x);
            float angle = utils.GetAngleFromVectorFloat(attackDirection);

            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            swordObject.transform.GetChild(0).transform.localPosition = attackDirection * dashAttackRange;
            swordObject.transform.GetChild(0).transform.localRotation = targetRotation;


            //swordObject.transform.GetChild(0).transform.localPosition = new Vector3(1f, 0, 0);
            swordObject.transform.GetChild(0).GetComponent<DamageSource>().RefreshDamageSource();

            //inputVector.x = swingMoveSpeed * playerFacingVector.x;

            animator.Play("attack", 0);
            
            

            t_attackLength = attackLength;
            t_attackCooldown = attackCooldown;
            //animator.SetTrigger("attack trigger");

            t_dashLockTime = dashLockTime;
            
            f_init_swordSwing = true;
        }

        //if(collisionDirections.y != -1 && moveStickVector.magnitude >= 0.25)
        if (moveStickVector.magnitude >= 0.25)
            inputVector.x = moveStickVector.normalized.x * moveSpeed;


        gravityVector = dashSpeed * attackDirection;
        inputVector = Vector2.zero;

        if (!swordObject.transform.GetChild(0).GetComponent<SwordScript>().animating)
            swordObject.SetActive(false);





        if (t_dashLockTime <= 0)
        {
            swordObject.SetActive(false);
            //if (moveStickVector.magnitude > 0.1f ||
            //        animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "attack")
            ChangeState(e_PlayerControllerStates.FreeMove);

        }

    }

    private void ProcessHiding()
    {
        if (hideTarget == null)
            ChangeState(e_PlayerControllerStates.FreeMove);
        else
        {
            inputVector = Vector2.zero;
            gravityVector = Vector2.zero;
            movementVector = Vector2.zero;
            lit = false;

            Vector2 distance = hideTarget.transform.position - transform.position;
            if (distance.magnitude > minZipDistance)
            {
                transform.Translate(distance.normalized * zipSpeed, Space.World);
            }
        }
        
    }

    private void ProcessPlatformGrab()
    {
        if (grabTarget == null)
            ChangeState(e_PlayerControllerStates.FreeMove);
        else
        {
            inputVector = Vector2.zero;
            gravityVector = Vector2.zero;
            movementVector = Vector2.zero;

            Vector2 distance = grabTarget.transform.position - transform.position;
            if (distance.magnitude > minGrabDistance)
            {
                transform.Translate(distance.normalized * zipSpeed, Space.World);
            }
            else transform.position = grabTarget.transform.position;

            
        }

    }


    private void ProcessStealthKill()
    {
        gravityVector = Vector2.zero;

        // check if the kill animation has finished
        if (!f_kozip && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "ko")
        {
            animator.SetBool("lock", false);
            currentTarget.SendMessage("KOEnd");
            currentTarget.SendMessage("StealthKilled");
            currentTarget = null;
            ChangeState(e_PlayerControllerStates.FreeMove);
        }

        // do initial things
        if (!f_init_stealthKill && currentTarget != null)
        {
            animator.Play("air dash", 0);
            animator.SetBool("lock", true);
            collisionDirections = Vector2.zero;
            f_init_stealthKill = true;
        }

        // move towards target until we are close
        Vector2 distance = currentTarget.transform.position - transform.position;
        if (distance.magnitude > minZipDistance)
        {
            transform.Translate(distance.normalized * zipSpeed, Space.World);
        }
        // once we are close
        // if we are eligible then start the animation
        // if facing direction matches or we're above
        else// if( KOTargetValid() )
        {
            //currentTarget.SendMessage("StealthKilled");
            //currentTarget = null;
            currentTarget.SendMessage("KOStart");
            f_kozip = false;
            animator.Play("ko", 0);
            inputVector = Vector2.zero;
        }/*
        else
        {
            Debug.Log("ko aborted");
            currentTarget.SendMessage("KOEnd");
            animator.SetBool("lock", false);
            currentTarget = null;
            ChangeState(e_PlayerControllerStates.FreeMove);
            //TriggerKnockback((int)currentTarget.GetComponent<EnemyStateMachine>().facingDirection);
        }*/
        
            


    }

    void ProcessBlink()
    {
        if (!f_init_blink)
        {
            tempMoveV = movementVector;
            tempGravityV = gravityVector;
            movementVector = Vector2.zero;
            gravityVector = Vector2.zero;
            spriteRenderer.enabled = false;
            f_init_blink = true;
        }

        lit = false;

        Vector2 distance = blinkPosition - transform.position;
        if (distance.magnitude > blinkSpeed * Time.deltaTime)
        {
            transform.Translate(distance.normalized * blinkSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.position = blinkPosition;
            movementVector = tempMoveV;
            gravityVector = tempGravityV;
            gravityVector.y += blinkAirBop;
            spriteRenderer.enabled = true;
            ChangeState(e_PlayerControllerStates.FreeMove);
        }

    }

    void ProcessHurt()
    {
        if (t_knockTime <= 0) ChangeState(e_PlayerControllerStates.FreeMove);
        float percentLeft = t_knockTime / knockTime;
        inputVector.x = percentLeft * knockVector.x * knockDirection;
    }

    void ProcessDead()
    {

    }





    void ApplyMovement()
    {

        movementVector.x = inputVector.x;
        movementVector.y = inputVector.y;

        // check if groundclose
        RaycastHit2D hit = Physics2D.BoxCast(
            collider.bounds.center, 
            new Vector2(collider.size.x*0.8f, colliderYscale/2), 
            0, 
            Vector3.down, 
            (0.1f * Mathf.Abs(gravityVector.y/jumpManager.maxFallSpeed)) + (colliderYscale * 0.6f),
            collisionMask
            );
        
        if (hit)
        {
            f_groundClose = true;
        }
        else f_groundClose = false;


        // apply gravity if not grounded
        if ((collisionDirections.y != -1 && ( CurrentPlayerState == e_PlayerControllerStates.FreeMove 
            || CurrentPlayerState == e_PlayerControllerStates.SwordSwing
            || CurrentPlayerState == e_PlayerControllerStates.DashAttack)) 
            || CurrentPlayerState == e_PlayerControllerStates.Hurt)
        {
            movementVector.x = gravityVector.x + inputVector.x;
            movementVector.y = gravityVector.y;

            jumpManager.CalculateGravity();

        }
        // else set gravity to zero
        else if (collisionDirections.y == -1) gravityVector.y = 0;

        //if (collisionDirections.y != -1) jumpManager.CalculateGravity();

        // apply shoving from enemies if need be
        if (f_insideEnemy && !sliding && CurrentPlayerState == e_PlayerControllerStates.FreeMove)
            movementVector += EnemyShove();

        ClampMovementForCollisions();

        Vector2 offset = collider.offset;
        if(collisionDirections.y != 0)
            offset = Vector2.zero;

        hit = Physics2D.BoxCast((Vector2)transform.position + offset/*new Vector2(0, -0.05f)*/, 
                                new Vector2(collider.bounds.size.x, collider.bounds.size.y)*0.9f, 
                                0, 
                                movementVector * Time.deltaTime, 
                                (movementVector * Time.deltaTime).magnitude,
                                collisionMask);
        if (!hit)
            transform.position += (Vector3)movementVector * Time.deltaTime;
        else
        {
            transform.position = hit.centroid;
            //movementVector = Vector3.zero;
        }
        

        // reset collision flags
        collisionDirections = Vector2.zero;
    }


    void TriggerStealthKill()
    {
        //currentTarget = GetKillTarget();
        animator.SetTrigger("ko trigger");
        koStartPos = transform.position;
        currentTarget.SendMessage("KOStart");
        f_kozip = true;
        ChangeState(e_PlayerControllerStates.StealthKill);
    }


    public void ChangeState(e_PlayerControllerStates state)
    {
        previousPlayerState = CurrentPlayerState;
        CurrentPlayerState = state;
        sliding = false;
        if (!crouchReleased)
        {
            t_slideCooldown = slideCooldown;
            crouchReleased = true;
        }

        if (previousPlayerState == e_PlayerControllerStates.StealthKill && currentTarget != null)
            currentTarget.SendMessage("KOEnd");
        if (previousPlayerState == e_PlayerControllerStates.SwordSwing || previousPlayerState == e_PlayerControllerStates.DashAttack)
            f_init_swordSwing = false;
        if (previousPlayerState == e_PlayerControllerStates.StealthKill)
            f_init_stealthKill = false;
        if (previousPlayerState == e_PlayerControllerStates.Blink)
            f_init_blink = false;
    }

   private Vector2 EnemyShove()
    {
        if (shovingEnemy == null) return Vector2.zero;
        Vector3 antitarget = shovingEnemy.transform.position;
        Vector3 diff = transform.position - antitarget;
        diff.y = 0;

        return (Vector2) diff * shoveForce;


    }



    // Collisions and Triggers

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collisionLayers.Contains(collision.collider.gameObject.tag))
        {
            Vector2 normal;
            
            // these values get reset every frame, so checked again every frame
            for(int i = 0; i < collision.contacts.Length; i++)
            {
                normal = collision.contacts[i].normal;

                // if surface faces up
                if(Vector2.Angle(normal, Vector2.up) < 45f)
                {
                    collisionDirections.y = -1;
                    t_gracetimePostCollide = gracetimePostCollide;

                    
                    //
                }
                // if surface faces down
                else if (Vector2.Angle(normal, Vector2.down) < 45f)
                {
                    collisionDirections.y = 1;
                    //gravityVector.y = gravityVector.y * (1f - (0.7f * Time.deltaTime));
                    //gravityVector.y = 0;
                }
                // if surface faces left
                else if (Vector2.Angle(normal, Vector2.left) < 45f)
                {
                    collisionDirections.x = 1;
                }
                // if surface faces right
                else if (Vector2.Angle(normal, Vector2.right) < 45f)
                {
                    collisionDirections.x = -1;
                }


            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collisionLayers.Contains(collision.collider.gameObject.tag))
        {
            Vector2 normal;

            for (int i = 0; i < collision.contacts.Length; i++)
            {
                normal = collision.contacts[i].normal;

                // if surface faces up
                if (Vector2.Angle(normal, Vector2.up) < 45f)
                {
                    gravityVector.y = 0;
                    jumpManager.f_jumped = false;

                    if (t_gracetimePreCollide > 0)
                    {
                        collisionDirections.y = 0;
                        jumpManager.Jump();
                    }
                    //Debug.Break();
                }
                // if surface faces down
                else if (Vector2.Angle(normal, Vector2.down) < 45f)
                {
                    /*
                    gravityVector.y = 0;
                    if(movementVector.y > 0)
                        movementVector.y = 0;
                    */
                    jumpManager.JumpReleased();
                    //Debug.Break();
                    //jumpManager.HeadBonk();
                }



            }
        }

        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyProjectile")
        {

            DamageSource dmg = collision.gameObject.GetComponent<DamageSource>();
            if (dmg != null)
            {
                if (dmg.applyToTags.Contains(gameObject.tag) && !dmg.hasHit)
                {
                    dmg.hasHit = true;
                    currentHP -= dmg.damageAmount;

                    int dir;
                    if (collision.gameObject.transform.position.x > transform.position.x)
                        dir = -1;
                    else
                        dir = 1;
                    TriggerKnockback(dir);

                }
            }
        }

        else if(collision.transform.name == "shove zone")
        {
            shovingEnemy = collision.gameObject;
            f_insideEnemy = true;
        }

        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light" && CurrentPlayerState != e_PlayerControllerStates.Hiding)
        {
            Vector2 origin = collision.gameObject.transform.position;
            Vector2 direction = (transform.position - collision.gameObject.transform.position).normalized;

            Debug.DrawRay(origin, direction);

            RaycastHit2D ray = Physics2D.Raycast(origin, direction, 100, lightCheckMask);

            if (ray.transform.gameObject.tag == "Player")
                lit = true;
            else lit = false;
        }

        if (collision.gameObject.tag == "GrabPlatform") grabTarget = collision.gameObject;


        // Interactables

        if (collision.gameObject.tag == "HidingPlace")
        {
            f_canInteract = true;
            interactTarget = collision.gameObject;
        }

        else if (collision.gameObject.tag == "GroundItem")
        {
            f_canInteract = true;
            interactTarget = collision.gameObject;
        }

        else if (collision.gameObject.tag == "Door")
        {
            f_canInteract = true;
            interactTarget = collision.gameObject;
        }

        else if (collision.gameObject.tag == "Coin")
        {
            f_canInteract = true;
            interactTarget = collision.gameObject;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light") lit = false;

        if (collision.gameObject.tag == "GrabPlatform") grabTarget = null;

        if (collision.gameObject.tag == "HidingPlace" ||
            collision.gameObject.tag == "GroundItem" ||
            collision.gameObject.tag == "Door" ||
            collision.gameObject.tag == "Coin")
        {
            f_canInteract = false;
            interactTarget = null;
        }
        else if (collision.transform.name == "shove zone")
        {
            f_insideEnemy = false;
        }
    }


    private bool KOTargetValid()
    {
        if (currentTarget != null) {

            // conditions:
            // above enemy OR
            // behind them, which means facing same direction AND they're in front of player

            // if above them
            if (koStartPos.y > currentTarget.transform.position.y + 1f)
            {
                return true;
            }

            else if (transform.position.y > currentTarget.transform.position.y + 1f)
                return true;

            // OR if facing same direction
            else if (currentTarget.GetComponent<EnemyStateMachine>().facingDirection == playerFacingVector.x)
            {
                // AND target is in front of player
                float offset = currentTarget.transform.position.x - transform.position.x;
                if (Math.Sign(offset) == playerFacingVector.x)
                    return true;
            }
                

        }

        return false;
    }

    private void ClampMovementForCollisions()
    {
        if (collisionDirections.y > 0) movementVector.y = Mathf.Clamp(movementVector.y, -100, 0);
        else if (collisionDirections.y < 0) movementVector.y = Mathf.Clamp(movementVector.y, 0, 100);

        if (collisionDirections.x > 0) movementVector.x = Mathf.Clamp(movementVector.x, -100, 0);
        else if (collisionDirections.x < 0) movementVector.x = Mathf.Clamp(movementVector.x, 0, 100);
    }

    public void TriggerKnockback(int direction)
    {
        
        if (t_iTime <= 0)
        {
            t_knockTime = knockTime;
            gravityVector.y = knockVector.y;
            knockDirection = direction;
            ChangeState(e_PlayerControllerStates.Hurt);
        }
        
    }

    private void AimBlink()
    {
        // turn on aiming particle
        // place it at mousevector * distance
        blinkAimObject.SetActive(true);

        Vector2 aimVector = GetVectorToMouse().normalized * blinkRange;

        RaycastHit2D probe = Physics2D.BoxCast(transform.position, new Vector2(collider.bounds.size.x, collider.bounds.size.x), 0, aimVector, blinkRange, collisionMask);
        if (probe)
        {
            blinkAimObject.transform.position = probe.centroid;
        }
        else blinkAimObject.transform.position = transform.position + (Vector3)aimVector;
    }

    // manage animator variables
    private void UpdateAnimator()
    {
        if (collisionDirections.y == -1 || f_groundClose && CurrentPlayerState != e_PlayerControllerStates.WallGrab)
        {
            animator.SetBool("grounded", true);
            
        }
        else
        {
            animator.SetBool("grounded", false);
            
        }

        if (Mathf.Abs(moveStickVector.x) <= 0.5f) animator.SetBool("not moving", true);
        else animator.SetBool("not moving", false);

        if (sliding)
            animator.SetBool("sliding", true);
        else if (crouching)
        {
            animator.SetBool("crouching", true);
            animator.SetBool("sneaking", false);
            animator.SetBool("sliding", false);
        }
        else if (sneaking)
        {
            animator.SetBool("sneaking", true);
            animator.SetBool("crouching", false);
            animator.SetBool("sliding", false);
        }
        else
        {
            animator.SetBool("sneaking", false);
            animator.SetBool("crouching", false);
            animator.SetBool("sliding", false);
        }
            

        if(CurrentPlayerState == e_PlayerControllerStates.WallGrab) 
            animator.SetBool("wall grab", true);
        else animator.SetBool("wall grab", false);


        // attack
        if (CurrentPlayerState == e_PlayerControllerStates.SwordSwing ||
            CurrentPlayerState == e_PlayerControllerStates.StealthKill) {
            animator.SetBool("lock", true);
        }
        else animator.SetBool("lock", false);




        if (!animator.GetBool("wall grab") && !animator.GetBool("grounded") && !animator.GetBool("lock"))
        {
            animator.speed = 0;
            animator.Play("air", 0, GetAirFrame());
        }
        else
        {
            animator.speed = 1;
        }
    }

    private float GetAirFrame()
    {
        if (movementVector.y > 1.5)
            return 0;
        else if (movementVector.y < 1.5 && movementVector.y > -1.5)
            return 0.5f;
        else
            return 0.9f;

 
    }


    private void ShootProjectile(Projectile proj)
    {
        GameObject bullet =  Instantiate(baseProjectile, transform.position, Quaternion.identity);
        BaseProjectile p = bullet.GetComponent<BaseProjectile>();
        if (p != null) 
        {
            p.so_projectile = proj;
            p.launchVector = GetVectorToMouse();
            p.Setup();
        }

    }

    // Input Listener Methods
    void OnMove(InputValue value)
    {
        moveStickVector.x = value.Get<Vector2>().x;
        moveStickVector.y = value.Get<Vector2>().y;
        /*
        if(!backpack.open)
            moveStickVector.y = value.Get<Vector2>().y;*/

    }

    void OnJump(InputValue value)
    {

        jumpManager.f_jumpKeyDown = true;

        // if we're on the ground or platform grab
        if (collisionDirections.y == -1 || t_gracetimePostCollide > 0 || CurrentPlayerState == e_PlayerControllerStates.PlatformGrab)
        {
            if(CurrentPlayerState == e_PlayerControllerStates.PlatformGrab)
                ChangeState(e_PlayerControllerStates.FreeMove);
            collisionDirections.y = 0;
            jumpManager.Jump();
        }
        // else if in wall grab
        else if (CurrentPlayerState == e_PlayerControllerStates.WallGrab)
        {
            ChangeState(e_PlayerControllerStates.FreeMove);
            t_wallJumpNoGrabTime = wallJumpNoGrabTime;
            if (moveStickVector.y >= 0) jumpManager.WallJump();
            else if (moveStickVector.y < 0) jumpManager.WallJumpDown();
            collisionDirections.x = 0;
            playerFacingVector.x *= -1;
        }
        else
        {
            t_gracetimePreCollide = gracetimePreCollide;
        }

    }

    void OnAttack(InputValue value)
    {
        //if (currentTarget != null && CheckTargetLOS(currentTarget) && KOTargetValid())
            //TriggerStealthKill();
        //else
        //{
        if (t_attackCooldown <= 0 &&
        CurrentPlayerState == e_PlayerControllerStates.FreeMove &&
        !f_carryingObject)

            //ChangeState(e_PlayerControllerStates.SwordSwing);
            ChangeState(e_PlayerControllerStates.DashAttack);
        else if (f_carryingObject)
        {
            f_carryingObject = false;
            carriedObject.transform.position = transform.position;
            carriedObject.SendMessage("Dropped", GetVectorToMouse());
            carriedObject = null;
        }
        //}

        /*
        else if (equipList[activeEquipIndex] == e_Equipment.sword)
        {
            if (t_attackCooldown <= 0 &&
            CurrentPlayerState == e_PlayerControllerStates.FreeMove)

                ChangeState(e_PlayerControllerStates.SwordSwing);
        }*/
        


    }

    void OnAim(InputValue value)
    {
        if (controlScheme == e_ControlSchemes.Gamepad) aimStickVector = value.Get<Vector2>().normalized;
    }

    void OnInteract()
    {
        if (f_carryingObject)
        {
            f_carryingObject = false;
            carriedObject.transform.position = transform.position;
            carriedObject.SendMessage("Dropped", Vector2.zero);
            carriedObject = null;
        }

        else if (f_canInteract && CurrentPlayerState == e_PlayerControllerStates.FreeMove)
        {
            
            // bugs = false;
            if (interactTarget.tag == "HidingPlace")
            {
                hideTarget = interactTarget;
                ChangeState(e_PlayerControllerStates.Hiding);
                lit = false; 
            }

            else if (interactTarget.tag == "GroundItem")
            {
                // do item pickup stuff
                Destroy(interactTarget.gameObject);
                interactTarget = null;
                
            }

            else if (interactTarget.tag == "Coin")
            {
                carriedObject = interactTarget;
                f_carryingObject = true;
                carriedObject.SendMessage("PickedUp");
            }

            else if (interactTarget.tag == "Door")
            {
                interactTarget.SendMessage("ToggleOpen");
            }


        }
        else if(CurrentPlayerState == e_PlayerControllerStates.Hiding)
        {
            hideTarget = null;
            ChangeState(e_PlayerControllerStates.FreeMove);
        }
    }
   
    void OnToggleSneak(InputValue value)
    {
        //if(CurrentPlayerState == e_PlayerControllerStates.FreeMove)
        //{
            if(f_holdToRun) sneaking = false;
            else sneaking = !sneaking;
        //}

    }

    void OnSneakRelease(InputValue value)
    {
        if (f_holdToRun)
        {
            sneaking = true;
            
        }
    }

    void OnGamepadAny(InputValue value)
    {
        if (controlScheme != e_ControlSchemes.Gamepad) controlScheme = e_ControlSchemes.Gamepad;
    }

    void OnKeyboardAny(InputValue value)
    {
        if (controlScheme != e_ControlSchemes.MouseKeyboard) controlScheme = e_ControlSchemes.MouseKeyboard;
    }

    void OnChargeUp(InputValue value)
    {
        //currentTarget = GetKillTarget();
        /*
        if (CurrentPlayerState == e_PlayerControllerStates.FreeMove ||
            CurrentPlayerState == e_PlayerControllerStates.WallGrab)
        {
            f_isCharging = true;
            koIndicator.SetActive(true);
            if (currentTarget != null)
            {
                koIndicator.GetComponent<KOIndicator>().targetPosition = currentTarget.transform.position;
            }
            else koIndicator.GetComponent<KOIndicator>().targetPosition = transform.position + new Vector3(0, 1, 0);



            koIndicator.GetComponent<KOIndicator>().animationPercent = 0;
        }
         */
        if (equipList[activeEquipIndex] == e_Equipment.blink)
        {
            f_blinkAiming = true;
        }
        else if( dict_projectiles_enumSO.ContainsKey( equipList[activeEquipIndex] ))
        {
            ShootProjectile(dict_projectiles_enumSO[equipList[activeEquipIndex]]);
        }


                /*
        else if (equipList[activeEquipIndex] == e_Equipment.bearTrap)
        {
            ShootProjectile(so_bearTrap);
        }

        else if (equipList[activeEquipIndex] == e_Equipment.arrow)
        {
            ShootProjectile(so_arrow);
        }*/

    }

    void OnChargeRelease(InputValue value)
    {
        /*
        f_isCharging = false;
        koIndicator.SetActive(false);
        if (t_killChargeTime > killChargeTime && CheckTargetLOS(currentTarget)) TriggerStealthKill();
        */
        if (equipList[activeEquipIndex] == e_Equipment.blink && f_blinkAiming)
        {
            Debug.Log("Shbloink!");
            f_blinkAiming = false;
            blinkPosition = blinkAimObject.transform.position;
            if (CurrentPlayerState == e_PlayerControllerStates.FreeMove || CurrentPlayerState == e_PlayerControllerStates.WallGrab)
                ChangeState(e_PlayerControllerStates.Blink);
        }
    }

    void OnBagToggle(InputValue value)
    {
        backpack.SendMessage("ToggleOpen");
    }

    void OnEquipUp(InputValue value)
    {
        activeEquipIndex -= 1;
        if(activeEquipIndex < 0)
            activeEquipIndex = equipList.Length - 1;
        itembar.SetIndicator(activeEquipIndex);
    }

    void OnEquipDown(InputValue value)
    {
        activeEquipIndex += 1;
        if(activeEquipIndex > equipList.Length - 1)
            activeEquipIndex = 0;
        itembar.SetIndicator(activeEquipIndex);
    }

    
    void OnMeow()
    {
        GameObject noise = Instantiate(noisePrefab, transform.position, Quaternion.identity);
        noise.SendMessage("SetProfile", meowSound);
    }

    void KOTarget()
    {
        if (killzoneObject.GetComponent<StealthKillZone>().currentTarget == null)
            currentTarget = null;
        else if(CheckTargetLOS(killzoneObject.GetComponent<StealthKillZone>().currentTarget) && 
                CurrentPlayerState != e_PlayerControllerStates.StealthKill)
            currentTarget = killzoneObject.GetComponent<StealthKillZone>().currentTarget;
        
    }

    private bool CheckTargetLOS(GameObject target)
    {

        Vector2 origin = transform.position;
        Vector2 direction = (target.transform.position - transform.position).normalized;

        Debug.DrawRay(origin, direction);

        RaycastHit2D ray = Physics2D.Raycast(origin, direction, 100, koCheckMask);

        if (ray.transform.gameObject.tag == "Enemy")
        {
            return true;
        }
            
        else return false;
        
    }



    // Get variable methods
    public Vector2 GetVector2Input(string type)
    {
        switch (type)
        {
            case "aimStick":
                return aimStickVector;
        }
        return Vector2.zero;
    }

    // Returns a normalised vector of the direction from the player to the mouse position
    Vector2 GetVectorToMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - (Vector2)transform.position;
        return direction.normalized;
    }

    private void OnDrawGizmos() {

        if (Application.isPlaying)
        {
            RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + new Vector2(0, 0f),
                                new Vector2(collider.bounds.size.x, collider.bounds.size.y) * 0.9f,
                                0,
                                movementVector * Time.deltaTime,
                                (movementVector * Time.deltaTime).magnitude,
                                collisionMask);

            Gizmos.DrawWireCube(hit.centroid, new Vector2(collider.bounds.size.x, collider.bounds.size.y) * 0.9f);
        }

        
    }


}
