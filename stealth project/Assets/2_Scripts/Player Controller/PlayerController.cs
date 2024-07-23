using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public enum e_PlayerControllerStates
{
    FreeMove,
    SwordSwing,
    WallGrab,
    Hiding,
    Hurt,
    StealthKill,
    Dead
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

    [Header("Free Move")]
    public bool f_holdToRun = true;
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
    //public float climbSpeed = 3f;
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


    [Header("Hiding")]
    //public GameObject hidingPlace;

    public GameObject interactTarget;
    public GameObject hideTarget;

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

    [Header("Collisions")]
    public Vector2 collisionDirections = Vector2.zero; // set to 0 for no collision, -1 for left, 1 for right
    public string[] collisionLayers;
    public Vector2 groundNormal = Vector2.zero;
    public float colliderYModifier = 0.6f;
    private float colliderYscale = 0;

    public GameObject normalIndicator;
    public bool slopeCheckRaycast = false;
    public LayerMask slopeMask;
    public LayerMask collisionMask;
    public LayerMask lightCheckMask;
    public LayerMask koCheckMask;
    public float slopeRaycastDistance = 1;
    public Sound footstepSound;

    public UI_Backpack backpack;



    //private BoxCollider2D collider;
    //public GameObject colliderObject;
    private PlayerJumpManager jumpManager;
    private Animator animator;
    public GameObject graphicsObject;
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
    public float blinkRange = 3f;





    void Start()
    {
        //collider = colliderObject.GetComponent<BoxCollider2D>();
        jumpManager = GetComponent<PlayerJumpManager>();
        spriteRenderer = graphicsObject.GetComponent<SpriteRenderer>();
        defaultColour = spriteRenderer.color;
        animator = graphicsObject.GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        killZone = killzoneObject.GetComponent<StealthKillZone>();
        backpack = GameObject.Find("Backpack").GetComponent<UI_Backpack>();
        colliderYscale = collider.size.y;
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
            case e_PlayerControllerStates.Hurt:
                ProcessHurt();
                break;
            case e_PlayerControllerStates.Hiding:
                ProcessHiding();
                break;
            case e_PlayerControllerStates.StealthKill:
                ProcessStealthKill();
                break;
        }


        if(currentTarget != null)
        {
            koIndicator.SetActive(true);
            koIndicator.GetComponent<KOIndicator>().targetPosition = currentTarget.transform.position;
        }
        else
            koIndicator.SetActive(false);


        if (CurrentPlayerState != e_PlayerControllerStates.Hiding)
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

        if (t_gracetimePostCollide > 0) t_gracetimePostCollide -= Time.deltaTime;
        if (t_gracetimePreCollide > 0) t_gracetimePreCollide -= Time.deltaTime;
        if (t_wallJumpNoGrabTime > 0) t_wallJumpNoGrabTime -= Time.deltaTime;
        if (t_attackCooldown > 0) t_attackCooldown -= Time.deltaTime;
        if (t_knockTime > 0) t_knockTime -= Time.deltaTime;
        if (t_noiseInterval > 0) t_noiseInterval -= Time.deltaTime;
        if (t_attackLength > 0) t_attackLength -= Time.deltaTime;
        if (t_slideCooldown > 0) t_slideCooldown -= Time.deltaTime;
        if (t_iTime > 0) t_iTime -= Time.deltaTime;


    }

    private void Update()
    {
        //if (lit) spriteRenderer.color = defaultColour;
        //else spriteRenderer.color = darkColour;
        UpdateAnimator();
        if (collisionDirections.y == -1 || f_groundClose) collider.size = new Vector2(collider.size.x, colliderYscale);
        else
        {
            collider.size = new Vector2(collider.size.x, colliderYscale * colliderYModifier);
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
            t_slideCooldown = slideCooldown;
        }

        

        
        // we stop sliding if we change direction or jump or speed reaches zero (or change state do that elseswhere)
        if(Math.Sign(moveStickVector.x) != slideDirection ||
            moveStickVector.x == 0 ||
            collisionDirections.y != -1 ||
            !crouching)
        {
            sliding = false;
            
        }

        // check if we started sliding
        if (crouching && !sliding && crouchReleased && t_slideCooldown <= 0)
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
            if(!sneaking && !crouching && !f_isCharging && !sliding) 
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

            Vector3 snap = new Vector3(transform.position.x - (transform.position.x % gridDistance),
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
            swordObject.SetActive(true);
            swordObject.transform.position = transform.position;
            swordObject.transform.localScale = new Vector3( playerFacingVector.x, 1, 1 );
            swordObject.GetComponentInChildren<Animator>().SetTrigger("swing");
            swordObject.GetComponentInChildren<SwordScript>().animating = true;
            swordObject.transform.GetChild(0).transform.localPosition = new Vector3 (1f,0,0);
            swordObject.transform.GetChild(0).GetComponent<DamageSource>().RefreshDamageSource();
            inputVector.x = swingMoveSpeed * playerFacingVector.x;

            t_attackLength = attackLength;
            t_attackCooldown = attackCooldown;
            //animator.SetTrigger("attack trigger");
            animator.Play("attack", 0);

            f_init_swordSwing = true;
        }

        // clamp to zero when its close
        if (inputVector.magnitude <= 0.1) inputVector = Vector2.zero;

        if (inputVector.magnitude > 0)
        {
            inputVector.x = inputVector.x - (inputVector.x * swingMoveDecay * Time.deltaTime);
        }

        gravityVector.y = 0;

        if (!swordObject.GetComponentInChildren<SwordScript>().animating)
            swordObject.SetActive(false);

        
        if(t_attackLength <= 0)
        {
            swordObject.SetActive(false);
            if(     moveStickVector.magnitude > 0.1f ||
                    animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "attack") 
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


    private void ProcessStealthKill()
    {
        if (currentTarget == null && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "ko")
        {
            animator.SetBool("lock", false);
            ChangeState(e_PlayerControllerStates.FreeMove);
        }

        if (!f_init_stealthKill && currentTarget != null)
        {
            animator.Play("air dash", 0);
            animator.SetBool("lock", true);
            collisionDirections = Vector2.zero;
            f_init_stealthKill = true;
        }

        Vector2 distance = currentTarget.transform.position - transform.position;
        if (distance.magnitude > minZipDistance)
        {
            transform.Translate(distance.normalized * zipSpeed, Space.World);
        }
        else if(currentTarget.GetComponent<EnemyStateMachine>().facingDirection == playerFacingVector.x ||
            transform.position.y > currentTarget.transform.position.y - 0f)
        {
            currentTarget.SendMessage("StealthKilled");
            currentTarget = null;
            animator.Play("ko", 0);
        }
        else
        {
            Debug.Log("ko aborted");
            currentTarget = null;
            TriggerKnockback((int)currentTarget.GetComponent<EnemyStateMachine>().facingDirection);
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
        /*
        float targetSpeed = inputVector.x;

        if(movementVector.x != targetSpeed)
        {
            float delta = movementVector.x + targetSpeed;
            movementVector.x += delta * acceleration * Time.deltaTime;
        }

        if(Mathf.Abs(movementVector.x) > Mathf.Abs(targetSpeed)) movementVector.x = targetSpeed;
        */


        movementVector.x = inputVector.x;
        movementVector.y = inputVector.y;


        RaycastHit2D hit = Physics2D.BoxCast(
            collider.bounds.center, 
            new Vector2(collider.size.x*0.8f, colliderYscale), 
            0, 
            Vector3.down, 
            0.1f * Mathf.Abs(gravityVector.y/jumpManager.maxFallSpeed) + 0.3f,
            collisionMask
            );
        
        if (hit)
        {
            f_groundClose = true;
        }
        else f_groundClose = false;

        // apply gravity if not grounded
        if ((collisionDirections.y != -1 && CurrentPlayerState == e_PlayerControllerStates.FreeMove) 
            || CurrentPlayerState == e_PlayerControllerStates.Hurt)
        {
            movementVector.x = gravityVector.x + inputVector.x;
            movementVector.y = gravityVector.y;
            
        }
        // else set gravity to zero
        else if (collisionDirections.y == -1) gravityVector.y = 0;

        if (collisionDirections.y != -1) jumpManager.CalculateGravity();

        // apply shoving from enemies if need be
        if (f_insideEnemy && !sliding && CurrentPlayerState == e_PlayerControllerStates.FreeMove)
            movementVector += EnemyShove();

        ClampMovementForCollisions();

        transform.position += (Vector3)movementVector * Time.deltaTime;

        // reset collision flags
        collisionDirections = Vector2.zero;
    }


    void TriggerStealthKill()
    {
        //currentTarget = GetKillTarget();
        animator.SetTrigger("ko trigger");
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
        

        if (previousPlayerState == e_PlayerControllerStates.SwordSwing)
            f_init_swordSwing = false;
        if (previousPlayerState == e_PlayerControllerStates.StealthKill)
            f_init_stealthKill = false;
    }

   private Vector2 EnemyShove()
    {
        if (shovingEnemy == null) return Vector2.zero;
        Debug.Log("enemy shove");
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

                    
                    //gravityVector.y = 0;
                }
                // if surface faces down
                else if (Vector2.Angle(normal, Vector2.down) < 45f)
                {
                    collisionDirections.y = 1;
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
                }
                // if surface faces down
                else if (Vector2.Angle(normal, Vector2.down) < 45f)
                {
                    gravityVector.y = 0;
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

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light") lit = false;

        if (collision.gameObject.tag == "HidingPlace" ||
            collision.gameObject.tag == "GroundItem")
        {
            f_canInteract = false;
            interactTarget = null;
        }
        else if (collision.transform.name == "shove zone")
        {
            f_insideEnemy = false;
        }
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



    // manage animator variables
    private void UpdateAnimator()
    {
        if (collisionDirections.y == -1 || f_groundClose)
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






    // Input Listener Methods
    void OnMove(InputValue value)
    {
        moveStickVector.x = value.Get<Vector2>().x;
        if(!backpack.open)
            moveStickVector.y = value.Get<Vector2>().y;

    }

    void OnJump(InputValue value)
    {

        jumpManager.f_jumpKeyDown = true;

        // if we're on the ground
        if (collisionDirections.y == -1 || t_gracetimePostCollide > 0)
        {
            collisionDirections.y = 0;
            jumpManager.Jump();
        }
        // else if in wall grab
        else if (CurrentPlayerState == e_PlayerControllerStates.WallGrab)
        {
            ChangeState(e_PlayerControllerStates.FreeMove);
            t_wallJumpNoGrabTime = wallJumpNoGrabTime;
            if (inputVector.y >= 0) jumpManager.WallJump();
            else if (inputVector.y < 0) jumpManager.WallJumpDown();
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
        if (currentTarget != null && CheckTargetLOS(currentTarget))
            TriggerStealthKill();

        else if (t_attackCooldown <= 0 && 
            CurrentPlayerState == e_PlayerControllerStates.FreeMove &&
            collisionDirections.y == -1)

            ChangeState(e_PlayerControllerStates.SwordSwing);
    }

    void OnAim(InputValue value)
    {
        if (controlScheme == e_ControlSchemes.Gamepad) aimStickVector = value.Get<Vector2>().normalized;
    }

    void OnInteract()
    {
        if(f_canInteract && CurrentPlayerState == e_PlayerControllerStates.FreeMove)
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
                backpack.AddItem(interactTarget);
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
    }

    void OnChargeRelease(InputValue value)
    {
        /*
        f_isCharging = false;
        koIndicator.SetActive(false);
        if (t_killChargeTime > killChargeTime && CheckTargetLOS(currentTarget)) TriggerStealthKill();
        */
    }

    void OnBagToggle(InputValue value)
    {
        backpack.SendMessage("ToggleOpen");
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




}
