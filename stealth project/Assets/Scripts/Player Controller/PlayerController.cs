using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public enum PlayerControllerStates
{
    FreeMove,
    Attacking,
    //GrappleCast,
    //zipToTarget,
    //Dash,
    Hover,
    Hurt,
    Dead
}

public enum ControlSchemes
{
    MouseKeyboard,
    Gamepad
}

public class PlayerController : MonoBehaviour
{

    

    [SerializeField]
    private PlayerControllerStates CurrentPlayerState = PlayerControllerStates.FreeMove;
    private PlayerControllerStates previousPlayerState = PlayerControllerStates.FreeMove;

    [SerializeField]
    private ControlSchemes controlScheme = ControlSchemes.MouseKeyboard;

    private PlayerAttackManager playerAttacks;

    public bool lit = false;
    

    [Header("Free Move")]
    public float moveSpeed = 1;
    public float moveDecay = 10;
    public Vector2 inputVector = new Vector2(0, 0); // the vector being used as a handle from the user input to the movement vector
    public Vector2 movementVector = new Vector2(0, 0); // vector that is used to store the desired movement, before any checks
    private Vector2 moveStickVector = new Vector2(0, 0); // raw input from left stick / wasd
    private Vector2 aimStickVector = new Vector2(0, 0); // raw input from right stick
    private Vector2 playerFacingVector = new Vector2(1, 0); // used to aim abilities if there is no input, and used to determine sprite facing
    public Boolean groundedFlag = false; // tracks wether we're on the ground. turned on by colliding with ground. turned off by jumping
    public float gravityAccel = 1;
    public float maxFallSpeed = 10;
    public float groundDistance = 1; // How far away to place the player from the ground when grounded
    public int horizCollision = 0; // set to 0 for no collision, -1 for left, 1 for right
    public int vertiCollision = 0; // set to 0 for no collision, -1 for down, 1 for up


    [Header("Jump")]
    public float jumpForce = 5;

    public LayerMask selfLayerMask;

    // the attack state has some important variables that go with it. 
    // there is the InitFlag which is used to execute the initialisation code once when the state is entered
    // there are timers for how long it should be in the state for and also the cooldown before it can be entered again
    // these things exist for the dash state as well

    // cooldowns are managed in the update function
    
    /*
    [Header("Dash")]
    public float dashSpeed = 10;
    // init flag
    private bool InitFlagDash = false;
    // can dash flag
    private bool FlagCanDash = true;
    // lifespan timer
    public float dashTime = 0.3f;
    private float TimerDash = 0;
    // cooldown timer
    public float dashCooldownTime = 0.1f;
    private float TimerDashCooldown = 0;
    // percentage of the dash to wait before decelerating
    public float dashDecelPercentage = 0.2f;
    public float dashDecaySpeed = 10;
    // how long the plaer has to dbl tap and enter boost
    public float boostTapTime = 0.3f;
    private float TimerBoostTap = 0;
    */

    [Header("Hover")]
    public float hoverSpeed = 10;
    public float hoverTurnSpeed = 1;
    // init flag
    private bool InitFlaghover = false;
    // can dash flag
    private bool FlagCanhover = true;

    /*
    [Header("Grapple")]
    public float grappleMaxLength = 5;
    public float grappleMaxPullFactor = 2;
    public float grappleKnockbackForce = 5;
    public float grappleKnockbackDecay = 10;
    public float smallObjPullForce = 5;
    // init flag
    private bool InitFlagGrapple = false;
    // can dash flag
    private bool FlagCanGrapple = true;
    // lifespan timer
    public float grappleTime = 0.3f;
    private float TimerGrapple = 0;
    // cooldown timer
    public float grappleCooldownTime = 0.1f;
    private float TimerGrappleCooldown = 0;
    public GameObject currentTarget;
    private bool FlagHasGrappleTarget = false;*/
    /*
    [Header("ZipToTarget")]
    public float zipMoveSpeed = 10;
    public float zipCancelDistance = 1;
    private bool InitFlagZipToTarget = false;
    public float zipMaxTime = 2;
    private float TimerZipMax = 0;
    public float zipMaxInputAngle = 30;*/

    // when grapple is cast, player is sent backwards a tiny bit
    // a raycast is made in the 
    // the trigger is activated and a list is pulled of everything 
    /*
    [Header("Other Stuff")]
    public GameObject grappleHitBox;
    public float grappleHitBoxOffset = 9;
    public GameObject grappleLineObject;
    private GrappleLineRenderer grappleLineRenderer;*/



    void Start()
    {
        //grappleLineRenderer = grappleLineObject.GetComponent<GrappleLineRenderer>();
        //swordHitboxScript = SwordSwingObject.GetComponent<SwordHitboxScript>();
        playerAttacks = GetComponent<PlayerAttackManager>();


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyMovement();
        if (controlScheme == ControlSchemes.MouseKeyboard) aimStickVector = GetVectorToMouse();

        // if we have a current grapple target, make sure it's getting pulled
        /*if (FlagHasGrappleTarget)
        {
            AssignTargetPosToGrappleTarget();
        }*/

    }

    private void Update()
    {
        /*
        if (TimerDashCooldown > 0) TimerDashCooldown -= Time.deltaTime;
        else FlagCanDash = true;*/
        /*
        if (TimerGrappleCooldown > 0) TimerGrappleCooldown -= Time.deltaTime;
        else FlagCanGrapple = true;*/


        // place the grapple hitbox
        Vector2 grappleDirection = aimStickVector;
        if (aimStickVector == Vector2.zero) grappleDirection = playerFacingVector;
        //grappleHitBox.transform.localPosition = grappleDirection.normalized * grappleHitBoxOffset;
        //grappleHitBox.transform.right = grappleDirection.normalized;


        // Process the current state
        switch (CurrentPlayerState)
        {
            case PlayerControllerStates.FreeMove:
                ProcessFreeMove();
                break;
            case PlayerControllerStates.Attacking:
                playerAttacks.ProcessAttack();
                break;
            /*case PlayerControllerStates.Dash:
                ProcessDash();
                break;*/
            case PlayerControllerStates.Hover:
                ProcessHover();
                break;
            /*case PlayerControllerStates.GrappleCast:
                ProcessGrapple();
                break;
            case PlayerControllerStates.zipToTarget:
                ProcessZipToTarget();
                break;*/

        }

        
    }

    void ApplyMovement()
    {
        // todo: add collision detection stuff maybe if needed
        movementVector.x = inputVector.x;


        if(vertiCollision > 0) movementVector.y = Mathf.Clamp(movementVector.y, -100, 0);
        else if (vertiCollision < 0) movementVector.y = Mathf.Clamp(movementVector.y, 0, 100);

        if (horizCollision > 0) movementVector.x = Mathf.Clamp(movementVector.x, -100, 0);
        else if (horizCollision < 0) movementVector.x = Mathf.Clamp(movementVector.x, 0, 100);

        transform.position += (Vector3)movementVector * Time.deltaTime;
    }








    void ProcessFreeMove()
    {
        //Vector2 joystickVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        if (moveStickVector.magnitude >= 0.25)
        {
            inputVector.x = moveStickVector.normalized.x * moveSpeed;
            playerFacingVector = moveStickVector.normalized;
        }
        // if there is no input then apply movement decay
        else if (inputVector.magnitude > 0)
        {
            inputVector.x = inputVector.x - (inputVector.x * moveDecay * Time.deltaTime);
        }
        // clamp to zero when its close
        if (inputVector.magnitude <= 0.1) inputVector = Vector2.zero;

        // apply gravity if not grounded
        if (!groundedFlag)
        {
            movementVector.y -= gravityAccel * Time.deltaTime;
            if(movementVector.y < -maxFallSpeed) movementVector.y = -maxFallSpeed;
        }
        else
        {
            

            movementVector.y = 0;
            /*
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1, selfLayerMask);

            Ray ray = new Ray(transform.position, Vector2.down);
            Debug.DrawRay(ray.origin, ray.direction, Color.cyan);

            Debug.Log(hit.transform.gameObject.name);
            
            Vector2 pos = transform.position;
            pos.y = hit.point.y + groundDistance;
            transform.position = hit.point;
            */
        }


    }

    /*
    void ProcessDash()
    {
        // Initialise
        if (!InitFlagDash)
        {
            InitFlagDash = true;
            TimerDash = dashTime;
            TimerBoostTap = boostTapTime;

            // set input vector
            if (inputVector == Vector2.zero) inputVector = playerFacingVector;
            Vector2 dashDirection = inputVector;
            if (inputVector == Vector2.zero) inputVector = playerFacingVector;
            inputVector = dashDirection.normalized * dashSpeed;
        }

        TimerDash -= Time.deltaTime;
        if (TimerBoostTap > 0) TimerBoostTap -= Time.deltaTime;

        // apply decay after time has passed
        if(TimerDash < dashTime * dashDecelPercentage)
        {
            inputVector = inputVector - (inputVector * dashDecaySpeed * Time.deltaTime);
        }


        // Deinitialise
        if(TimerDash <= 0)
        {
            InitFlagDash = false;
            ChangeState(PlayerControllerStates.FreeMove);
            TimerDashCooldown = dashCooldownTime;
            FlagCanDash = false;
        }
    }*/

    void ProcessHover()
    {
        if(moveStickVector.magnitude > 0.25)
        {
            // playerfacingvector 
            // get the sum, normalised, times turn speed
            // add that t facing vector

            Vector2 sum = (moveStickVector + playerFacingVector).normalized * hoverTurnSpeed;
            playerFacingVector = (playerFacingVector + (sum * Time.deltaTime)).normalized;
        }

        inputVector = playerFacingVector * hoverSpeed;


    }

    /*
    void ProcessGrapple()
    {
        // Initialise
        if (!InitFlagGrapple)
        {
            InitFlagGrapple = true;

            // get a new target if we don't have one
            if (!FlagHasGrappleTarget)
            {
                // set timer for the state
                TimerGrapple = grappleTime;
                // if no aimstick vector use facing vector instead
                if (aimStickVector == Vector2.zero) aimStickVector = playerFacingVector;

                

                // get target for grapple
                currentTarget = grappleHitBox.GetComponent<GrappleHitboxScript>().GetTarget();
                // if there's a valid target
                if (currentTarget != null)
                {
                    // turn on the line renderer
                    grappleLineRenderer.FlagDraw = true;
                    Transform[] points = { transform, currentTarget.transform };
                    grappleLineRenderer.SetUpLine(points);

                    FlagHasGrappleTarget = true;

                    // if its a smallobject then give it a target position
                    if (currentTarget.gameObject.tag == "SmallObject")
                    {
                        currentTarget.GetComponent<SmallObjectScript>().grappled = true;
                        AssignTargetPosToGrappleTarget();
                    }

                    // if its and enemy then tell it to get grappled please
                    if(currentTarget.gameObject.tag == "Enemy")
                    {
                        currentTarget.gameObject.SendMessage("TriggerGrappled");
                    }
                    
                }
            }

            // if there's already target then do stuff 
            else
            {
                // if it's a small object then give it a pull
                if(currentTarget.gameObject.tag == "SmallObject")
                {
                    // get the controlling script
                    SmallObjectScript smlobj = currentTarget.GetComponent<SmallObjectScript>();
                    // get the direction of toward player
                    Vector2 direction = transform.position - currentTarget.transform.position;
                    smlobj.AddForce(direction.normalized, smallObjPullForce);
                    smlobj.grappled = false;
                }

                if(currentTarget.gameObject.tag == "Enemy")
                {
                    currentTarget.gameObject.SendMessage("TriggerForce", gameObject);
                }

                // then clear the target
                grappleLineRenderer.FlagDraw = false;
                FlagHasGrappleTarget = false;
            }
            
        }

        TimerGrapple -= Time.deltaTime;


        // Deinitialise
        if (TimerGrapple <= 0)
        {
            InitFlagGrapple = false;
            ChangeState(previousPlayerState);
            TimerGrappleCooldown = grappleCooldownTime;
            FlagCanGrapple = false;

        }
    }*/

    /*
    void ProcessZipToTarget()
    {
        // initialise
        if (!InitFlagZipToTarget)
        {
            InitFlagZipToTarget = true;
            Vector2 targetDirection = (currentTarget.transform.position - transform.position).normalized;
            inputVector = targetDirection * zipMoveSpeed;

            TimerZipMax = zipMaxTime;
        }

        float targetDistance = (currentTarget.transform.position - transform.position).magnitude;

        TimerZipMax -= Time.deltaTime;

        if (targetDistance <= zipCancelDistance || TimerZipMax <= 0)
        {
            InitFlagZipToTarget = false;
            FlagHasGrappleTarget = false;
            grappleLineRenderer.FlagDraw = false;

            if (currentTarget.gameObject.tag == "SmallObject")
            {
                // get the controlling script
                SmallObjectScript smlobj = currentTarget.GetComponent<SmallObjectScript>();
                smlobj.grappled = false;
            }

            ChangeState(PlayerControllerStates.FreeMove);
        }
    }*/

    void ProcessHurt()
    {

    }



    void ProcessDead()
    {

    }







    // assigns a position for the target
    // used to simulate dragging the object
    /*
    void AssignTargetPosToGrappleTarget()
    {
        if (currentTarget.gameObject.tag == "SmallObject")
        {
            // get the distance to it
            Vector2 distance = currentTarget.transform.position - transform.position;
            // if its larger than max
            if (distance.magnitude > grappleMaxLength)
            {
                // get the target position using the max length
                Vector2 targetPos = distance.normalized * grappleMaxLength;

                // set the position
                //currentTarget.transform.position = transform.position + (Vector3)targetPos;

                currentTarget.GetComponent<SmallObjectScript>().smoothTarget = transform.position + (Vector3)targetPos;
            }
            else
            {
                currentTarget.GetComponent<SmallObjectScript>().smoothTarget = currentTarget.transform.position;
            }
        }
    }*/


    public void ChangeState(PlayerControllerStates state)
    {
        previousPlayerState = CurrentPlayerState;
        CurrentPlayerState = state;
    }

    // Returns a normalised vector of the direction from the player to the mouse position
    Vector2 GetVectorToMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - (Vector2)transform.position;
        return direction.normalized;
    }


    // Get variable methods
    public Vector2 GetVector2Input(string type)
    {
        switch (type) {
            case "aimStick":
                return aimStickVector;
        }
        return Vector2.zero;
    }

    public Vector2 GetPlayerFacing()
    {
        return playerFacingVector;
    }


    /*
    public void GrappleBreak(GameObject instigator)
    {
        if (FlagHasGrappleTarget)
        {
            if(instigator == currentTarget)
            {
                grappleLineRenderer.FlagDraw = false;
                FlagHasGrappleTarget = false;
            }
        }
        
        
    }*/













    // Input Listener Methods

    void OnMove(InputValue value)
    {
        moveStickVector.x = value.Get<Vector2>().x;
    }

    void OnJump(InputValue value)
    {
        Debug.Log("Jump");

        if (groundedFlag)
        {
            movementVector.y += jumpForce;
            groundedFlag = false;
        }
        
    }

    void OnAim(InputValue value)
    {
        if(controlScheme == ControlSchemes.Gamepad) aimStickVector = value.Get<Vector2>().normalized;
    }


    /*
    void OnAttack(InputValue value)
    {
        if(CurrentPlayerState == PlayerControllerStates.FreeMove && playerAttacks.FlagCanMeleeAttack)
        {
            ChangeState(PlayerControllerStates.Attacking);
            playerAttacks.currentAttack = PlayerAttacks.ThreeHitCombo;
            playerAttacks.FlagNewAttackInput = true;
        }
        else if (CurrentPlayerState == PlayerControllerStates.Attacking && playerAttacks.currentAttack == PlayerAttacks.ThreeHitCombo)
        {
            playerAttacks.FlagNewAttackInput = true;
        }

        
        // if we're in a dash or zip then kick
        if(CurrentPlayerState == PlayerControllerStates.Dash || CurrentPlayerState == PlayerControllerStates.zipToTarget)
        {
            ChangeState(PlayerControllerStates.Attacking);
            playerAttacks.currentAttack = PlayerAttacks.Kick;
            playerAttacks.FlagNewAttackInput = true;
        }
    }
    
    void OnAttackHold(InputValue value)
    {
        if(CurrentPlayerState == PlayerControllerStates.FreeMove || CurrentPlayerState == PlayerControllerStates.Attacking)
        {
            if (playerAttacks.currentAttack == PlayerAttacks.ThreeHitCombo)
            {
                ChangeState(PlayerControllerStates.Attacking);
                playerAttacks.currentAttack = PlayerAttacks.ChargeUp;
                playerAttacks.FlagHoldAttackTriggered = true;
            }

            else if (playerAttacks.currentAttack == PlayerAttacks.HeavySlash)
            {
                playerAttacks.FlagHoldAttackTriggered = true;
            }

        }
    
    }*/
    /*
    void OnUnAttack(InputValue value)
    {
        if (CurrentPlayerState == PlayerControllerStates.Attacking && playerAttacks.currentAttack == PlayerAttacks.ChargeUp)
        {
            playerAttacks.FlagChargeReleaseInput = true;
        }
    }*/
    /*
    void OnDash(InputValue value)
    {
        Debug.Log("Dash");

        
        if (CurrentPlayerState == PlayerControllerStates.FreeMove && FlagCanDash)
        {
            
            if (FlagHasGrappleTarget)
            {
                ChangeState(PlayerControllerStates.zipToTarget);
            }
            else ChangeState(PlayerControllerStates.Dash);
        }

        else if (CurrentPlayerState == PlayerControllerStates.Attacking && playerAttacks.currentAttack == PlayerAttacks.ChargeUp)
        {
            playerAttacks.InitFlagChargeUp = false;
            playerAttacks.currentAttack = PlayerAttacks.None;
            ChangeState(PlayerControllerStates.Dash);
        }

    }
    /*
    void OnDashHold(InputValue value)
    {
        if (CurrentPlayerState == PlayerControllerStates.Dash)
        {
            if (TimerBoostTap <= 0)
            {
                InitFlagDash = false;
                TimerDashCooldown = dashCooldownTime;
                FlagCanDash = false;
                ChangeState(PlayerControllerStates.Hover);
            }
        }
    }
    
    void OnUnDash()
    {
        Debug.Log("Undash");
        if (CurrentPlayerState == PlayerControllerStates.Hover)
        {
            ChangeState(PlayerControllerStates.FreeMove);
        }
    }
    
    void OnGrapple(InputValue value)
    {
        if ((CurrentPlayerState == PlayerControllerStates.FreeMove || CurrentPlayerState == PlayerControllerStates.Hover)
            && FlagCanGrapple)
        {
            ChangeState(PlayerControllerStates.GrappleCast);
        }
    }
    */

    void OnGamepadAny(InputValue value)
    {
        if (controlScheme != ControlSchemes.Gamepad) controlScheme = ControlSchemes.Gamepad;
    }

    void OnKeyboardAny(InputValue value)
    {
        if (controlScheme != ControlSchemes.MouseKeyboard) controlScheme = ControlSchemes.MouseKeyboard;
    }





    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.tag == "Ground")
        {
            groundedFlag = true;

            Vector2 point;

            /*
            for(int i = 0; i < collision.contactCount; i++)
            {
                point = collision.GetContact(i).point;

                if (point.y > transform.position.y)
                {
                    vertiCollision = 1;
                }
                else if (point.y < transform.position.y)
                {
                    vertiCollision = -1;
                }


                if (point.x > transform.position.x)
                {
                    horizCollision = 1;
                }
                else if (point.x < transform.position.x)
                {
                    horizCollision = -1;
                }

            }*/


   

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
        if (collision.collider.gameObject.tag == "Ground")
        {
            groundedFlag = false;
        }

        vertiCollision = 0;
        horizCollision = 0;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light") lit = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light") lit = false;
    }


}
