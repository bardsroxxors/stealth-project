using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public class EnemyPatrolMovement : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed = 5;
    public float jumpCheckOffset = 1;
    public Vector2 movementVector;
    public bool isGrounded = false;

    [Header("Patrol Behaviour")]
    public float nodeWaitTime = 3f;
    public int currentPatrolNode = 0;
    public GameObject patrolRoute;
    public float nextNodeDistance = 0.5f;

    [Header("Pathfinding")]
    public Transform targetPosition;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;
    public float nextWaypointDistance = 1f;


    private Path path;
    private int currentWaypoint = 0;
    public Transform[] patrolRouteList;


    Seeker seeker;
    Rigidbody2D rb;



    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);


        patrolRouteList = patrolRoute.GetComponentsInChildren<Transform>();
        


        targetPosition = patrolRouteList[currentPatrolNode];

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PathFollow();
        PatrolRouteFollow();
    }


    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, targetPosition.position, OnPathComplete);
        }
    }


    // follows the current A* path
    private void PathFollow()
    {
        if (path == null) return;


        // reached end of path
        if(currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

      

        // direction calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        if (direction.x > 0) direction.x = 1;
        else if(direction.x < 0) direction.x = -1;

        direction.y = 0;
        movementVector = direction * moveSpeed;


        ApplyMovement();


        // next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

    }


    private void PatrolRouteFollow()
    {
        // next waypoint
        float distance = Vector2.Distance(rb.position, patrolRouteList[currentPatrolNode].position);
        if (distance < nextNodeDistance)
        {
            currentPatrolNode++;
        }
    }


    private void ApplyMovement()
    {
        transform.position += (Vector3)movementVector * Time.deltaTime;
    }


    private void OnPathComplete(Path p)
    {
        path = p;
        currentWaypoint = 0;
    }


}
