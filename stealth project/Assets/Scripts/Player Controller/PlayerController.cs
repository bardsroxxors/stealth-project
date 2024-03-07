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
    public Boolean groundedFlag = false; // tracks wether we're on the ground. turned on by colliding with ground. turned off by jumping
    public float gravityAccel = 1;
    public float maxFallSpeed = 10;
    public float groundDistance = 1; // How far away to place the player from the ground when grounded
    public int horizCollision = 0; // set to 0 for no collision, -1 for left, 1 for right
    public int vertiCollision = 0; // set to 0 for no collision, -1 for down, 1 for up
    public string[] collisionLayers;


    [Header("Jump")]
    public float jumpForce = 5;
    public LayerMask selfLayerMask;


    private Bounds boxBounds;
    




    void Start()
    {
        //grappleLineRenderer = grappleLineObject.GetComponent<GrappleLineRenderer>();
        //swordHitboxScript = SwordSwingObject.GetComponent<SwordHitboxScript>();
        playerAttacks = GetComponent<PlayerAttackManager>();

        boxBounds = GetComponent<BoxCollider2D>().bounds;


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
        if (/*!groundedFlag*/ vertiCollision != -1)
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














    // Input Listener Methods

    void OnMove(InputValue value)
    {
        moveStickVector.x = value.Get<Vector2>().x;
    }

    void OnJump(InputValue value)
    {
        Debug.Log("Jump");

        if (/*groundedFlag*/ vertiCollision == -1)
        {
            movementVector.y += jumpForce;
            //groundedFlag = false;
        }
        
    }

    void OnAim(InputValue value)
    {
        if(controlScheme == ControlSchemes.Gamepad) aimStickVector = value.Get<Vector2>().normalized;
    }


    void OnGamepadAny(InputValue value)
    {
        if (controlScheme != ControlSchemes.Gamepad) controlScheme = ControlSchemes.Gamepad;
    }

    void OnKeyboardAny(InputValue value)
    {
        if (controlScheme != ControlSchemes.MouseKeyboard) controlScheme = ControlSchemes.MouseKeyboard;
    }


    // called by the ground trigger at the players feet
    private void GroundTriggerContact()
    {
        //groundedFlag = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collisionLayers.Contains(collision.collider.gameObject.tag))
        {
            

            Vector2 point;
            Vector2 normal;

            //bool collisionUp
            
            for(int i = 0; i < collision.contacts.Length; i++)
            {
                //point = collision.contacts[i].point;
                normal = collision.contacts[i].normal;

                // if surface faces up at all
                if(Vector2.Angle(normal, Vector2.up) < 90f)
                {
                    vertiCollision = -1;
                    groundedFlag = true;
                }
                // if surface faces down at all
                else if (Vector2.Angle(normal, Vector2.down) < 90f)
                {
                    vertiCollision = 1;
                }

            }


   

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
        if (collisionLayers.Contains(collision.collider.gameObject.tag))
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
