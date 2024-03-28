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
    WallMove,
    Hover,
    Hurt,
    Dead
}

public enum e_ControlSchemes
{
    MouseKeyboard,
    Gamepad
}

public class PlayerController : MonoBehaviour
{



    [SerializeField]
    public e_PlayerControllerStates CurrentPlayerState = e_PlayerControllerStates.FreeMove;
    private e_PlayerControllerStates previousPlayerState = e_PlayerControllerStates.FreeMove;

    [SerializeField]
    private e_ControlSchemes controlScheme = e_ControlSchemes.MouseKeyboard;

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
    public Vector2 gravityVector = Vector2.zero;

    [Header("Wall Climbing")]
    public bool canWallClimb = true;
    public float climbSpeed = 3f;
    public Vector2 wallJumpForce = Vector2.zero;


    [Header("Sword Swing")]
    public GameObject swordObject;
    public float attackCooldown = 0.5f;
    private float t_attackCooldown = 0;
    private bool f_init_swordSwing = false;
    public float swingMoveSpeed = 5f;
    public float swingMoveDecay = 5f;


    [Header("Collisions")]
    public Vector2 collisionDirections = Vector2.zero; // set to 0 for no collision, -1 for left, 1 for right
    public string[] collisionLayers;
    public Vector2 groundNormal = Vector2.zero;

    public GameObject normalIndicator;
    public bool slopeCheckRaycast = false;
    public LayerMask slopeMask;
    public float slopeRaycastDistance = 1;


    //private BoxCollider2D collider;
    //public GameObject colliderObject;
    private PlayerJumpManager jumpManager;
    public GameObject graphicsObject;
    private SpriteRenderer spriteRenderer;
    private Color defaultColour;
    public Color darkColour;

    [Header("Grace Timers")]
    public float gracetimePostCollide = 0.2f;
    public float gracetimePreCollide = 0.2f;
    private float t_gracetimePostCollide = 0f;
    private float t_gracetimePreCollide = 0f;

    public float wallJumpNoGrabTime = 0.5f;
    private float t_wallJumpNoGrabTime = 0f;





    void Start()
    {
        //collider = colliderObject.GetComponent<BoxCollider2D>();
        jumpManager = GetComponent<PlayerJumpManager>();
        spriteRenderer = graphicsObject.GetComponent<SpriteRenderer>();
        defaultColour = spriteRenderer.color;
    }


    private void LateUpdate()
    {
        graphicsObject.transform.localScale = new Vector3(playerFacingVector.x, 1, 1);
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
            case e_PlayerControllerStates.WallMove:
                ProcessWallMove();
                break;
            case e_PlayerControllerStates.SwordSwing:
                ProcessSwordSwing();
                break;
        }

        ApplyMovement();



        // manage timers
        if (t_gracetimePostCollide > 0) t_gracetimePostCollide -= Time.deltaTime;
        if (t_gracetimePreCollide > 0) t_gracetimePreCollide -= Time.deltaTime;
        if (t_wallJumpNoGrabTime > 0) t_wallJumpNoGrabTime -= Time.deltaTime;
        if (t_attackCooldown > 0) t_attackCooldown -= Time.deltaTime;
    }

    private void Update()
    {
        //if (lit) spriteRenderer.color = defaultColour;
        //else spriteRenderer.color = darkColour;
    }




    // Update Functions for States

    void ProcessFreeMove()
    {

        //CheckSlopeRaycast();

        // get inputVector from raw input, set player facing
        if (moveStickVector.magnitude >= 0.25)
        {
            inputVector.x = moveStickVector.normalized.x * moveSpeed;
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



        // change to wall move under right conditions
        // if we are colliding with a wall and not the ground we are in wallMove state
        // and the wall jump grace timer has depleted
        if (collisionDirections.x != 0 && collisionDirections.y != -1 && t_wallJumpNoGrabTime <= 0 && canWallClimb)
        {
            ChangeState(e_PlayerControllerStates.WallMove);
        }

    }

    private void ProcessWallMove()
    {
        // get inputVector from raw input, set player facing 
        if (moveStickVector.magnitude >= 0.25)
        {
            inputVector.x = moveStickVector.normalized.x * moveSpeed;
            if(collisionDirections.x != 0)
                inputVector.y = moveStickVector.normalized.y * moveSpeed;

            playerFacingVector = moveStickVector.normalized;
        }

        // if there is no input then apply movement decay
        else if (inputVector.magnitude > 0)
        {
            inputVector.x = inputVector.x - (inputVector.x * moveDecay * Time.deltaTime);
            if (collisionDirections.x != 0)
                inputVector.y = inputVector.y - (inputVector.y * moveDecay * Time.deltaTime);
        }
        // clamp to zero when its close
        if (inputVector.magnitude <= 0.1) inputVector = Vector2.zero;


        // change to free move under right conditions
        // if we are not olliding with a wall or we're colliding with the ground we are in freeMove
        if (collisionDirections.x == 0 || collisionDirections.y == -1) CurrentPlayerState = e_PlayerControllerStates.FreeMove;
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
            swordObject.transform.GetChild(0).transform.localPosition = new Vector3 (0.75f,0,0);

            inputVector.x = swingMoveSpeed * playerFacingVector.x;

            t_attackCooldown = attackCooldown;

            f_init_swordSwing = true;
        }



        if (inputVector.magnitude > 0)
        {
            inputVector.x = inputVector.x - (inputVector.x * swingMoveDecay * Time.deltaTime);
        }

        gravityVector.y = 0;

        if (!swordObject.GetComponentInChildren<SwordScript>().animating)
        {
            swordObject.SetActive(false);
            ChangeState(e_PlayerControllerStates.FreeMove);
        }

    }


    void ProcessHurt()
    {

    }

    void ProcessDead()
    {

    }





    void ApplyMovement()
    {
        movementVector.x = inputVector.x;
        movementVector.y = inputVector.y;


        // apply gravity if not grounded
        if (collisionDirections.y != -1 && CurrentPlayerState == e_PlayerControllerStates.FreeMove)
        {
            //movementVector.x += gravityVector.x;
            movementVector.y = gravityVector.y;
            
        }
        // else set gravity to zero
        else if (collisionDirections.y == -1) gravityVector.y = 0;

        jumpManager.CalculateGravity();

        ClampMovementForCollisions();

        transform.position += (Vector3)movementVector * Time.deltaTime;

        // reset collision flags
        collisionDirections = Vector2.zero;
    }





    public void ChangeState(e_PlayerControllerStates state)
    {
        previousPlayerState = CurrentPlayerState;
        CurrentPlayerState = state;

        if (previousPlayerState == e_PlayerControllerStates.SwordSwing)
            f_init_swordSwing = false;
    }

    



    // Collisions and Triggers

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collisionLayers.Contains(collision.collider.gameObject.tag))
        {
            Vector2 normal;
            
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

   
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light") lit = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light") lit = false;
    }

    /*
    private void CheckSlopeRaycast()
    {
        RaycastHit2D ray = Physics2D.Raycast(
            collider.bounds.center + new Vector3(0, -collider.bounds.extents.y, 0),
            Vector2.down,
            slopeRaycastDistance,
            slopeMask

            );

        slopeCheckRaycast = ray;

        if (ray) groundNormal = ray.normal;
        else groundNormal = Vector2.up;

    }
    */
    private void ClampMovementForCollisions()
    {
        if (collisionDirections.y > 0) movementVector.y = Mathf.Clamp(movementVector.y, -100, 0);
        else if (collisionDirections.y < 0) movementVector.y = Mathf.Clamp(movementVector.y, 0, 100);

        if (collisionDirections.x > 0) movementVector.x = Mathf.Clamp(movementVector.x, -100, 0);
        else if (collisionDirections.x < 0) movementVector.x = Mathf.Clamp(movementVector.x, 0, 100);
    }









    // Input Listener Methods
    void OnMove(InputValue value)
    {
        moveStickVector.x = value.Get<Vector2>().x;
        moveStickVector.y = value.Get<Vector2>().y;
    }

    void OnJump(InputValue value)
    {
        Debug.Log("Jump");

        jumpManager.f_jumpKeyDown = true;

        if (collisionDirections.y == -1 || t_gracetimePostCollide > 0)
        {
            collisionDirections.y = 0;
            jumpManager.Jump();
        }
        else if (CurrentPlayerState == e_PlayerControllerStates.WallMove)
        {
            
            ChangeState(e_PlayerControllerStates.FreeMove);
            t_wallJumpNoGrabTime = wallJumpNoGrabTime;
            jumpManager.WallJump();
            collisionDirections.x = 0;
        }
        else
        {
            t_gracetimePreCollide = gracetimePreCollide;
        }

    }

    void OnAttack(InputValue value)
    {
        if (t_attackCooldown <= 0)
            ChangeState(e_PlayerControllerStates.SwordSwing);
    }

    void OnAim(InputValue value)
    {
        if (controlScheme == e_ControlSchemes.Gamepad) aimStickVector = value.Get<Vector2>().normalized;
    }

    void OnGamepadAny(InputValue value)
    {
        if (controlScheme != e_ControlSchemes.Gamepad) controlScheme = e_ControlSchemes.Gamepad;
    }

    void OnKeyboardAny(InputValue value)
    {
        if (controlScheme != e_ControlSchemes.MouseKeyboard) controlScheme = e_ControlSchemes.MouseKeyboard;
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
