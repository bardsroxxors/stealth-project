using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol_Guard_State : Guard_State
{


    [Header("Patrolling")]
    public GameObject patrolRouteObject;
    public int currentNodeIndex = 0;
    private Vector2 currentPatrolDestination;
    public float nodeCompleteDistance = 0.5f;
    public float patrolSpeed = 5;
    [HideInInspector]
    public float patrolFacing = -1f;

    
    [HideInInspector]
    public float t_currentWaitTimer = 0f;

    private PatrolRoute patrolRoute;
    private bool boomerangBackwards = false;


    void Start()
    {
        base.OnStart();

        FindNearestPatrolRoute();

        patrolRoute = patrolRouteObject.GetComponent<PatrolRoute>();
        if (patrolRoute != null)
        {
            //currentPatrolDestination = patrolRouteObject.transform;
            currentPatrolDestination = (Vector3)patrolRoute.nodes[currentNodeIndex] + patrolRouteObject.transform.position;
            awareScript.lastKnownPosition = currentPatrolDestination;
        }
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        if (cond.conditions.Contains(e_EnemyConditions.immobile))
        {
            t_currentWaitTimer = 1f;
            sm.ChangeStateEnum(e_EnemyStates.headSwivel);
        }

        float nodeDistance = ((Vector3)currentPatrolDestination - transform.position).magnitude;

        if (nodeDistance <= nodeCompleteDistance)
        {
            patrolFacing = patrolRoute.directions[currentNodeIndex];
            t_currentWaitTimer = patrolRoute.waitTimes[currentNodeIndex];
            SetNextNodeIndex();

            if (cond.conditions.Contains(e_EnemyConditions.vigilant))
                sm.ChangeStateEnum(e_EnemyStates.headSwivel);
            else
                sm.ChangeStateEnum(e_EnemyStates.waiting);

        }

        // look in the direction we're walking
        float angle = 190;
        if (em.facingDirection > 0) angle = 350;
        Vector3 lookTarget = Vector3.zero;

        lookTarget = sm.utils.GetVectorFromAngle(angle) * 2;
        lookTarget = transform.TransformPoint(lookTarget);

        sm.targetLookPosition = lookTarget;
        //targetLookPosition = transform.position + Vector3.Scale(defaultLookTarget.transform.localPosition, facingVector);
        sm.SightConeTrack();


        pathing.SetPathfindTarget((Vector3)patrolRoute.nodes[currentNodeIndex] + patrolRouteObject.transform.position);
        em.inputVector = pathing.PathDirection() * patrolSpeed;
    }


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

        currentPatrolDestination = (Vector3)patrolRoute.nodes[currentNodeIndex] + patrolRouteObject.transform.position;
        pathing.SetPathfindTarget(currentPatrolDestination);


        awareScript.lastKnownPosition = currentPatrolDestination;

    }

    void FindNearestPatrolRoute()
    {
        Debug.Log("Finding nearest route");
        GameObject[] routes = GameObject.FindGameObjectsWithTag("PatrolRoute");
        float minDist = 1000f;
        GameObject nearest = null;

        if (routes != null)
            foreach (GameObject route in routes)
            {
                float dist = Vector3.Distance(transform.position, route.transform.position);
                if (dist < minDist)
                {
                    nearest = route;
                    minDist = dist;
                }

            }

        patrolRouteObject = nearest;
    }
}
