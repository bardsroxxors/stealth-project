using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public enum EnemyStates
{
    patrolling,
    waiting,
    investigate
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
    public float facingDirection = 1;

    [Header("Patrolling")]
    public GameObject patrolRouteObject;
    public int currentNodeIndex = 0;
    private Transform currentPatrolDestination;
    public float nodeCompleteDistance = 0.5f;

    [Header("Waiting")]
    public float currentWaitTimer = 0f;

    [Header("Investigate")]
    public float investigateDistance = 1f;


    [Header("Pathfinding")]
    public Vector3 pathfindTarget;
    public float pathUpdateSeconds = 0.5f;
    public float nextWaypointDistance = 1f;

    [Header("Sight Cone")]
    public GameObject sightCone;
    public Vector2 sightConePosition = Vector2.zero;


    private Path path;
    private int currentWaypoint = 0;
    Seeker seeker;
    Rigidbody2D rb;
    private EnemyAwareness awareScript;




    private PatrolRoute patrolRoute;
    private bool boomerangBackwards = false;



    // Start is called before the first frame update
    void Start()
    {

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        awareScript = GetComponent<EnemyAwareness>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);


        patrolRoute = patrolRouteObject.GetComponent<PatrolRoute>();
        if (patrolRoute != null)
        {
            currentPatrolDestination = patrolRoute.nodes[currentNodeIndex];
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        inputVector = Vector3.zero;

        if (sightCone != null)
        {
            sightCone.transform.localPosition = sightConePosition;

            // change the direction of sight cone
            if (facingDirection != sightCone.transform.localScale.x) sightCone.transform.localScale = new Vector3(facingDirection, 1, 1);
        }

        switch (currentState)
        {
            case EnemyStates.patrolling:
                ProcessPatrolling();
                break;
            case EnemyStates.waiting:
                ProcessWaiting();
                break;
            case EnemyStates.investigate:
                ProcessInvestigate();
                break;
        }



        if (awareScript.currentAwareness == AwarenessLevel.curious) currentState = EnemyStates.investigate;

        ApplyMovement();

        // update facing direction
        if (movementVector.x != 0) facingDirection = Mathf.Sign(movementVector.x);

    }


    private void ProcessPatrolling()
    {
        float nodeDistance = (currentPatrolDestination.position - transform.position).magnitude;
        if (nodeDistance <= nodeCompleteDistance)
        {
            currentWaitTimer = patrolRoute.waitTimes[currentNodeIndex];
            SetNextNodeIndex();
            currentState = EnemyStates.waiting;
            //inputVector = Vector3.zero;
        }


        pathfindTarget = patrolRoute.nodes[currentNodeIndex].position;

        PathFollow();

        

    }

    private void ProcessWaiting()
    {

        if (currentWaitTimer <= 0) currentState = EnemyStates.patrolling;

        else currentWaitTimer -= Time.deltaTime;
    }

    private void ProcessInvestigate()
    {
        float dist = (awareScript.lastKnownPosition - transform.position).magnitude;
        if(dist > investigateDistance)
        {
            pathfindTarget = awareScript.lastKnownPosition;

            PathFollow();
        }
        else inputVector = Vector3.zero;


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
            seeker.StartPath(rb.position, pathfindTarget, OnPathComplete);
        }
    }

    // called when A* path is finished
    private void OnPathComplete(Path p)
    {
        path = p;
        currentWaypoint = 0;
    }



    // called by the awareness script when the state chanegs
    public void AwarenessChange(AwarenessLevel newState)
    {
        if (newState == AwarenessLevel.curious || newState == AwarenessLevel.alert)
        {
            currentState = EnemyStates.investigate;
        }

        else if (newState == AwarenessLevel.unaware) currentState = EnemyStates.patrolling;
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
