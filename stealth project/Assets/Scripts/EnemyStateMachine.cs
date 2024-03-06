using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public enum EnemyStates
{
    patrolling,
    waiting
}

public class EnemyStateMachine : MonoBehaviour
{

    public EnemyStates currentState = EnemyStates.patrolling;

    [Header("Movement")]
    public float moveSpeed = 5;
    public float moveDecay = 0.5f;
    public float gravity = 5;
    public Vector2 inputVector = Vector2.zero;
    public Vector2 movementVector = Vector2.zero;

    [Header("Patrolling")]
    public GameObject patrolRouteObject;
    public int currentNodeIndex = 0;
    private Transform currentPatrolDestination;
    public float nodeCompleteDistance = 0.5f;

    [Header("Waiting")]
    private float currentWaitTimer = 0f;


    [Header("Pathfinding")]
    public Transform pathfindTarget;
    public float pathUpdateSeconds = 0.5f;
    public float nextWaypointDistance = 1f;


    private Path path;
    private int currentWaypoint = 0;
    Seeker seeker;
    Rigidbody2D rb;




    private PatrolRoute patrolRoute;
    private bool boomerangBackwards = false;



    // Start is called before the first frame update
    void Start()
    {

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);


        patrolRoute = patrolRouteObject.GetComponent<PatrolRoute>();
        if(patrolRoute != null)
        {
            currentPatrolDestination = patrolRoute.nodes[currentNodeIndex];
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        switch (currentState)
        {
            case EnemyStates.patrolling:
                ProcessPatrolling();
                break;
            case EnemyStates.waiting:
                ProcessWaiting();
                break;
        }

        ApplyMovement();


    }


    private void ProcessPatrolling()
    {
        float nodeDistance = (currentPatrolDestination.position - transform.position).magnitude;
        if (nodeDistance <= nodeCompleteDistance)
        {
            currentWaitTimer = patrolRoute.waitTimes[currentNodeIndex];
            SetNextNodeIndex();
            currentState = EnemyStates.waiting;
            inputVector = Vector3.zero;
        }


        pathfindTarget = patrolRoute.nodes[currentNodeIndex];

        PathFollow();

    }

    private void ProcessWaiting()
    {

        if (currentWaitTimer <= 0) currentState = EnemyStates.patrolling;

        else currentWaitTimer -= Time.deltaTime;
    }



    private void ApplyMovement()
    {
        // apply input
        if (inputVector.x != 0) movementVector.x = inputVector.x;
        if (inputVector.y != 0) movementVector.y = inputVector.y;

        // apply decay
        if (inputVector.x == 0) movementVector.x = movementVector.x - (movementVector.x * moveDecay * Time.deltaTime);
        if (inputVector.y == 0) movementVector.y = movementVector.y - (movementVector.y * moveDecay * Time.deltaTime);

        //inputVector - (inputVector * moveDecay * Time.deltaTime)

        // clamp to zero
        if (Mathf.Abs(inputVector.x) <= 0.2) inputVector.x = 0;
        if (Mathf.Abs(inputVector.y) <= 0.2) inputVector.y = 0;

        transform.position += (Vector3)movementVector * Time.deltaTime;
    }




    // called periodically to update A* path based on target
    private void UpdatePath()
    {
        if (seeker.IsDone() && pathfindTarget != null)
        {
            seeker.StartPath(rb.position, pathfindTarget.position, OnPathComplete);
        }
    }

    // called when A* path is finished
    private void OnPathComplete(Path p)
    {
        path = p;
        currentWaypoint = 0;
    }




    



    // follows the current A* path
    private void PathFollow()
    {
        if (path == null) return;


        // reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }



        // direction calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        if (direction.x > 0) direction.x = 1;
        else if (direction.x < 0) direction.x = -1;

        direction.y = 0;
        inputVector = direction * moveSpeed;



        // next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

    }

    // set the index for the next patrol route node
    private void SetNextNodeIndex()
    {
        // change the index properly
        if (patrolRoute.boomerang)
        {
            if (currentNodeIndex == patrolRoute.nodes.Length - 1) boomerangBackwards = true;


            else if (currentNodeIndex == 0) boomerangBackwards = false;


            if (boomerangBackwards) currentNodeIndex--;
            else currentNodeIndex++;
        }

        currentPatrolDestination = patrolRoute.nodes[currentNodeIndex];


    }
}
