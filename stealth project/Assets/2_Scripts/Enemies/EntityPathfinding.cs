using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPathfinding : MonoBehaviour
{

    // this script should tell the movement script which way to go
    // talk to it using the inputVector

    [Header("Pathfinding")]


    private Vector3 pathfindTarget;
    public float pathUpdateSeconds = 0.5f;
    public float nextWaypointDistance = 1f;
    private Path path;
    private int currentWaypoint = 0;
    Seeker seeker;

    private Rigidbody2D rb;



    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {   /*
        if (followPath)
        {
            PathFollow();
        }*/
    }




    public Vector3 GetPathFindTarget()
    {
        return pathfindTarget;
    }


    // #######  Pathfinding Methods  #######

    // Moves along the current A* path *** old
    // Now it should return the direction required to path along path
    public Vector2 PathDirection()
    {
        if (path == null) return Vector2.zero;

        // if we can't reach our destination, ie we've accidentally reached the end of our path
        // right now just scramble
        if (currentWaypoint > path.vectorPath.Count - 1)
        {
            //Debug.Log("Scramblin' time!");
            UpdatePath();

            /*
            f_waitingToScramble = false;
            queuedState = e_EnemyStates.investigate;
            ChangeState(e_EnemyStates.scramble);*/

            return Vector2.zero;
        }


        // next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return Vector2.zero;
        }


        

        // direction calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        if (direction.x > 0) direction.x = 1;
        else if (direction.x < 0) direction.x = -1;

        direction.y = 0;



        return direction;

        /*
        if (currentState == e_EnemyStates.patrolling)
            ec_movement.inputVector = direction * patrolSpeed;
        else if (awareScript.currentAwareness != AwarenessLevel.alert)
            ec_movement.inputVector = direction * patrolSpeed * 1.5f;
        else
            ec_movement.inputVector = direction * pursueSpeed;
        */

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


    public void SetPathfindTarget(Vector3 target)
    {
        pathfindTarget = target;
        UpdatePath();
    }
    

    // #######  ------------------  #######
}
