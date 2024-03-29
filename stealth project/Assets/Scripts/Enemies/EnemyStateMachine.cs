using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using static UnityEngine.GraphicsBuffer;
using System;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;

public enum e_EnemyStates
{
    patrolling, // walking to next patrol point
    waiting,    // waiting at patrpol point
    investigate,// walking to last known position
    headSwivel  // looking around
}

public class EnemyStateMachine : MonoBehaviour
{

    public e_EnemyStates currentState = e_EnemyStates.patrolling;

    [Header("Movement")]
    public float patrolSpeed = 5;
    public float pursueSpeed = 8;
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
    public float t_currentWaitTimer = 0f;

    [Header("Investigate")]
    public float investigateDistance = 1f;

    [Header("Head Swivel")]
    public float swivelStateTime = 1.45f;
    public float swivelChangeTime = 0.5f;
    private float t_swivelStateTime = 0f;
    private float t_swivelChangeTime = 0f;
    //public float swivelWalkSpeed = 2f;
    //public float swivelWalkTime = 0.5f;
    private float t_swivelWalkTime = 0f;
    private bool f_init_swivel = false;
    public float randomPointDistance = 2f;
    public float[] lookAngles;


    [Header("Pathfinding")]
    public Vector3 pathfindTarget;
    public float pathUpdateSeconds = 0.5f;
    public float nextWaypointDistance = 1f;

    [Header("Sight Cone")]
    public GameObject sightCone;
    public Vector2 sightConePosition = Vector2.zero;
    public float coneTrackingSpeed = 1;
    public Vector3 targetLookPosition = Vector3.zero;
    public GameObject defaultLookTarget;
    private Vector3 fuckyou = Vector3.zero;


    private Path path;
    private int currentWaypoint = 0;
    Seeker seeker;
    Rigidbody2D rb;
    private EnemyAwareness awareScript;
    private Utilities utils = new Utilities();




    private PatrolRoute patrolRoute;
    private bool boomerangBackwards = false;



    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        awareScript = GetComponent<EnemyAwareness>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

        fuckyou = new Vector3(facingDirection, 1, 1);

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

        switch (currentState)
        {
            case e_EnemyStates.patrolling:
                ProcessPatrolling();
                break;
            case e_EnemyStates.waiting:
                ProcessWaiting();
                break;
            case e_EnemyStates.investigate:
                ProcessInvestigate();
                break;
            case e_EnemyStates.headSwivel:
                ProcessHeadSwivel();
                break;
        }

        //if (awareScript.currentAwareness == AwarenessLevel.curious) currentState = e_EnemyStates.investigate;

        ApplyMovement();

        // update facing direction
        if (movementVector.x != 0)  facingDirection = Mathf.Sign(movementVector.x);

        fuckyou = new Vector3(facingDirection, 1, 1);



        if (sightCone != null)  sightCone.transform.localPosition = sightConePosition;

        // manage timers
        if(t_swivelChangeTime > 0) t_swivelChangeTime -= Time.deltaTime;
        if (t_swivelStateTime > 0) t_swivelStateTime -= Time.deltaTime;
        if (t_swivelWalkTime > 0) t_swivelWalkTime -= Time.deltaTime;
        if (t_currentWaitTimer > 0) t_currentWaitTimer -= Time.deltaTime;
    }









    // Update Functions for States
    private void ProcessPatrolling()
    {
        float nodeDistance = (currentPatrolDestination.position - transform.position).magnitude;
        if (nodeDistance <= nodeCompleteDistance)
        {
            t_currentWaitTimer = patrolRoute.waitTimes[currentNodeIndex];
            SetNextNodeIndex();
            ChangeState(e_EnemyStates.headSwivel);
            //inputVector = Vector3.zero;
        }

        targetLookPosition = transform.position + Vector3.Scale(defaultLookTarget.transform.localPosition, fuckyou);
        SightConeTrack();


        pathfindTarget = patrolRoute.nodes[currentNodeIndex].position;

        PathFollow();

    }

    private void ProcessWaiting()
    {
        if (t_currentWaitTimer <= 0) ChangeState(e_EnemyStates.patrolling);

        SightConeTrack();
    }

    private void ProcessInvestigate()
    {
        float dist = (awareScript.lastKnownPosition - transform.position).magnitude;

        targetLookPosition = awareScript.lastKnownPosition;
        SightConeTrack();

        if (dist > investigateDistance)
        {
            pathfindTarget = awareScript.lastKnownPosition;

            PathFollow();
        }
        else inputVector = Vector3.zero;

    }


    private void ProcessHeadSwivel()
    {
        if (!f_init_swivel)
        {
            f_init_swivel = true;
            t_swivelStateTime = swivelStateTime;
            t_swivelChangeTime = 0;
        }

        Vector3 lookTarget = Vector3.zero;

        // pathfind target = random vector normalised * distance

        if(t_swivelChangeTime <= 0)
        {
            t_swivelChangeTime = swivelChangeTime;
            float angle = lookAngles[Random.Range(0, lookAngles.Length)];

            lookTarget = utils.GetVectorFromAngle(angle) * randomPointDistance;
            lookTarget = transform.TransformPoint(lookTarget);

            targetLookPosition = lookTarget;
        }


        float dist = (lookTarget - transform.position).magnitude;
        SightConeTrack();
        /*
        if (dist > investigateDistance)
        {
            pathfindTarget = lookTarget;
            PathFollow();
        }
        else*/ inputVector = Vector3.zero;

        if (t_swivelStateTime <= 0)
        {
            f_init_swivel = false;
            ChangeState(e_EnemyStates.patrolling);
        }


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

    public void ChangeState(e_EnemyStates state)
    {
        currentState = state;
    }


    // move sight cone towards target
    private void SightConeTrack()
    {
        if (targetLookPosition != null)
        {
            Vector2 direction = targetLookPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            sightCone.transform.rotation = Quaternion.Slerp(sightCone.transform.rotation, targetRotation, coneTrackingSpeed * Time.deltaTime);
        }
    }



    // called by the awareness script when the state chanegs
    public void AwarenessChange(AwarenessLevel newState)
    {
        if (newState == AwarenessLevel.curious || newState == AwarenessLevel.alert)
        {
            ChangeState(e_EnemyStates.investigate);
        }

        else if (newState == AwarenessLevel.unaware) ChangeState(e_EnemyStates.patrolling);
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










    // Pathfinding Methods

    // Moves along the current A* path
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
        if(currentState == e_EnemyStates.patrolling)
            inputVector = direction * patrolSpeed;
        else if(currentState != e_EnemyStates.patrolling)
            inputVector = direction * pursueSpeed;


        // next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

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

    



}
