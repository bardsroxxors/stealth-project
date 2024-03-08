using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    

    [Header("Jump")]
    public float jumpForce = 5;
    public LayerMask selfLayerMask;
    public float gravityAccel = 1;
    public float maxFallSpeed = 10;


    [Header("Collisions")]
    public Vector2 collisionDirections = Vector2.zero; // set to 0 for no collision, -1 for left, 1 for right
    public string[] collisionLayers;
    public Vector2 groundNormal = Vector2.zero;

    public GameObject normalIndicator;
    public bool slopeCheckRaycast = false;
    public LayerMask slopeMask;
    public float slopeRaycastDistance = 1;


    private BoxCollider2D collider;
    public GameObject colliderObject;




    void Start()
    {
        //grappleLineRenderer = grappleLineObject.GetComponent<GrappleLineRenderer>();
        //swordHitboxScript = SwordSwingObject.GetComponent<SwordHitboxScript>();
        playerAttacks = GetComponent<PlayerAttackManager>();

        collider = colliderObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyMovement();
        if (controlScheme == ControlSchemes.MouseKeyboard) aimStickVector = GetVectorToMouse();



    }

    private void Update()
    {



        // Process the current state
        switch (CurrentPlayerState)
        {
            case PlayerControllerStates.FreeMove:
                ProcessFreeMove();
                break;

        }

        
    }

    void ApplyMovement()
    {
        movementVector.x = inputVector.x;

        Vector2 groundDirection = Vector2.Perpendicular(groundNormal) * -1 * inputVector.x;

        if (Vector2.Angle(groundNormal, Vector2.up) >= 20 && slopeCheckRaycast)
        {

            movementVector.x = groundDirection.x;
            if(movementVector.y <= jumpForce/4)
                movementVector.y = groundDirection.y;

            
        }

        normalIndicator.transform.right = groundDirection;


        // apply gravity if not grounded
        if (collisionDirections.y != -1 && CurrentPlayerState == PlayerControllerStates.FreeMove)
        {
            movementVector.y -= gravityAccel * Time.deltaTime;
            if (movementVector.y < -maxFallSpeed) movementVector.y = -maxFallSpeed;
        }

        ClampMovementForCollisions();

        transform.position += (Vector3) movementVector * Time.deltaTime;

        collisionDirections = Vector2.zero;
    }








    void ProcessFreeMove()
    {
        //Vector2 joystickVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        CheckSlopeRaycast();

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

        


    }


    void ProcessHurt()
    {

    }



    void ProcessDead()
    {

    }



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
    }

    void OnJump(InputValue value)
    {
        Debug.Log("Jump");

        if ( collisionDirections.y == -1)
        {
            movementVector.y = jumpForce;
        }
        
    }

    

    private void CheckSlopeRaycast()
    {
        RaycastHit2D ray = Physics2D.Raycast(
            collider.bounds.center + new Vector3(0, -collider.bounds.extents.y, 0),
            Vector2.down,
            slopeRaycastDistance,
            slopeMask

            );

        slopeCheckRaycast = ray;

        if(ray) groundNormal = ray.normal;
        else groundNormal = Vector2.up;

    }

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
                    //groundNormal = normal;
                }
                // if surface faces down
                else if (Vector2.Angle(normal, Vector2.down) < 45f)
                {
                    collisionDirections.y = 1;
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
    /*
    private void OnCollisionExit2D(Collision2D collision)
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
                    collisionDirections.y = 0;
                }
                // if surface faces down
                else if (Vector2.Angle(normal, Vector2.down) < 45f)
                {
                    collisionDirections.y = 0;
                }
                // if surface faces left
                else if (Vector2.Angle(normal, Vector2.left) < 45f)
                {
                    collisionDirections.x = 0;
                }
                // if surface faces right
                else if (Vector2.Angle(normal, Vector2.right) < 45f)
                {
                    collisionDirections.x = 0;
                }


            }
        }

    }*/


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light") lit = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light") lit = false;
    }










    void OnAim(InputValue value)
    {
        if (controlScheme == ControlSchemes.Gamepad) aimStickVector = value.Get<Vector2>().normalized;
    }


    void OnGamepadAny(InputValue value)
    {
        if (controlScheme != ControlSchemes.Gamepad) controlScheme = ControlSchemes.Gamepad;
    }

    void OnKeyboardAny(InputValue value)
    {
        if (controlScheme != ControlSchemes.MouseKeyboard) controlScheme = ControlSchemes.MouseKeyboard;
    }


}
