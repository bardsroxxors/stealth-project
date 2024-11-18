using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    public GameObject graphicsObject;

    [Header("Movement")]
    public float patrolSpeed = 5;
    public float pursueSpeed = 8;
    public float moveDecay = 0.5f;
    public float gravity = 5;

    public Vector2 inputVector = Vector2.zero;
    [SerializeField]
    private Vector2 movementVector = Vector2.zero;
    [SerializeField]
    private Vector2 gravityVector = Vector2.zero;

    public float facingDirection = 1;
    public float facingSwitchTimer = 0.2f;
    private float t_facingSwitchTimer = 0;
    public string[] collisionLayers;
    [SerializeField]
    private Vector2 collisionDirections = Vector2.zero;
    public float maxFallSpeed = 20f;
    public float gravityAccel = 1f;
    public GameObject shovingEnemy;
    public bool f_insideEnemy = false;
    public float shoveForce = 3f;

    


    [Header("Jump")]
    private bool f_jumpInit = false;
    public Vector2 jumpMinDistance = new Vector2(2, 0.8f);
    
    public Vector2 jumpForce;



    private Vector3 facingVector = Vector3.zero;
    private EnemyStateMachine ec_main;


    // Start is called before the first frame update
    void Start()
    {
        facingVector = new Vector3(facingDirection, 1, 1);
        ec_main = GetComponent<EnemyStateMachine>();
        
    }

    // Update is called once per frame
    void Update()
    {
        ManageTimers();

        if (graphicsObject.transform.localScale.x != facingDirection)
        {
            graphicsObject.transform.localScale = new Vector3(facingDirection, 1, 1);
            attackTrigger.transform.localPosition = new Vector3(facingDirection, 0, 0);
        }
    }

    private void FixedUpdate()
    {
        inputVector = Vector3.zero;


        ApplyMovement();

        // update facing direction
        if (Mathf.Abs(movementVector.x) > 0.1) SwitchFacing(Mathf.Sign(movementVector.x));

        facingVector = new Vector3(facingDirection, 1, 1);


        
    }


    private void ApplyMovement()
    {
        // apply input
        if (inputVector.x != 0) movementVector.x = inputVector.x;
        if (inputVector.y != 0) movementVector.y = inputVector.y;


        // apply gravity if not grounded
        if (collisionDirections.y != -1 || gravityVector.x != 0)
        {
            movementVector.x = gravityVector.x + inputVector.x;
            movementVector.y = gravityVector.y;

        }
        // else set gravity to zero
        else if (collisionDirections.y == -1) gravityVector.y = 0;

        // apply decay
        if (inputVector.x == 0) movementVector.x = movementVector.x - (movementVector.x * moveDecay * Time.deltaTime);
        if (inputVector.y == 0) movementVector.y = movementVector.y - (movementVector.y * moveDecay * Time.deltaTime);


        // clamp to zero
        if (Mathf.Abs(inputVector.x) <= 0.2) inputVector.x = 0;
        if (Mathf.Abs(inputVector.y) <= 0.2) inputVector.y = 0;

        if (Mathf.Abs(movementVector.x) <= 0.1) movementVector.x = 0;
        if (Mathf.Abs(movementVector.y) <= 0.1) movementVector.y = 0;

        if (collisionDirections.y != -1 || gravityVector.x != 0) CalculateGravity();

        /*
        // apply shoving from enemies if need be
        if (f_insideEnemy && (currentState == e_EnemyStates.investigate ||
                              currentState == e_EnemyStates.patrolling))
            movementVector += EnemyShove();*/

        //movementVector += EnemyShove();

        ClampMovementForCollisions();

        if (!conditions.Contains(e_EnemyConditions.immobile))
            transform.position += (Vector3)movementVector * Time.deltaTime;
    }


    public void ApplyForce(Vector2 force)
    {
        gravityVector += force;
    }

    public void CalculateGravity()
    {

        if (true)
        {


            gravityVector.y -= gravityAccel * Time.deltaTime;

            // keep fall speed below the max
            if (gravityVector.y < -maxFallSpeed) gravityVector.y = -maxFallSpeed;


            // decay x component as well
            if (collisionDirections.y != -1)
                gravityVector.x = gravityVector.x - (Mathf.Sign(gravityVector.x) * 2 * Time.deltaTime);
            else
                gravityVector.x = gravityVector.x - (gravityVector.x * moveDecay * Time.deltaTime);

            // clamp x to zero when its close
            if (Mathf.Abs(gravityVector.x) <= 0.1) gravityVector.x = 0;
        }


    }

    private Vector2 EnemyShove()
    {
        if (shovingEnemy == null) return Vector2.zero;
        Vector3 antitarget = shovingEnemy.transform.position;
        Vector3 diff = transform.position - antitarget;
        diff.y = 0;

        return (Vector2)diff * shoveForce;


    }

    private void ClampMovementForCollisions()
    {
        if (collisionDirections.y > 0) movementVector.y = Mathf.Clamp(movementVector.y, -100, 0);
        else if (collisionDirections.y < 0) movementVector.y = Mathf.Clamp(movementVector.y, 0, 100);

        if (collisionDirections.x > 0) movementVector.x = Mathf.Clamp(movementVector.x, -100, 0);
        else if (collisionDirections.x < 0) movementVector.x = Mathf.Clamp(movementVector.x, 0, 100);
    }

    public void Jump()
    {
        gravityVector = jumpForce;
        gravityVector.x *= facingDirection;
        collisionDirections = Vector2.zero;
    }

    public void SwitchFacing(float newFacing)
    {
        if (t_facingSwitchTimer <= 0)
        {
            facingDirection = newFacing;
            t_facingSwitchTimer = facingSwitchTimer;
        }
    }



    public Vector2 GetMovementVector()
    {
        return movementVector; 
    }

    public Vector2 GetCollisionDirections()
    {
        return collisionDirections;
    }


    private void ManageTimers()
    {
        if (t_facingSwitchTimer > 0) t_facingSwitchTimer -= Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.name == "shove zone" && !collision.transform.IsChildOf(transform))
        {
            shovingEnemy = collision.gameObject;
            f_insideEnemy = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.name == "shove zone" && !collision.transform.IsChildOf(transform))
        {
            f_insideEnemy = false;
        }
        /*if(collision.transform.tag == "Door")
        {
            collision.transform.gameObject.SendMessage("SetOpen", SendMessageOptions.DontRequireReceiver);
        }*/
    }

    private void OnCollisionStay2D(Collision2D collision)
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
                    collisionDirections.y = -1;
                }
                // if surface faces down
                else if (Vector2.Angle(normal, Vector2.down) < 45f)
                {
                    if (collisionDirections.y != 1)
                        gravityVector.y = 0;
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collisionLayers.Contains(collision.collider.gameObject.tag))
        {
            collisionDirections = Vector2.zero;
        }
    }

}
