using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using static UnityEngine.GraphicsBuffer;
using System;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;
using System.Linq;

public enum e_EnemyStates
{
    patrolling, // walking to next patrol point
    waiting,    // waiting at patrpol point
    investigate,// walking to last known position
    headSwivel, // looking around
    reaction,   // reacting to new information
    damageFlinch,// reacting to minor damage
    attacking   // normal attack
}

public enum e_EnemyConditions
{
    vigilant // Looks around frequently

}

public class EnemyStateMachine : MonoBehaviour
{

    public e_EnemyStates currentState = e_EnemyStates.patrolling;
    private e_EnemyStates previousState;
    public int currentHP = 10;
    public int maxHP = 10;
    public GameObject graphicsObject;
    private Animator animator;
    public List<e_EnemyConditions> conditions = new List<e_EnemyConditions>();

    [Header("Movement")]
    public float patrolSpeed = 5;
    public float pursueSpeed = 8;
    public float moveDecay = 0.5f;
    public float gravity = 5;
    public Vector2 inputVector = Vector2.zero;
    public Vector2 movementVector = Vector2.zero;
    public float facingDirection = 1;
    public float facingSwitchTimer = 0.2f;
    private float t_facingSwitchTimer = 0;


    [Header("Patrolling")]
    public GameObject patrolRouteObject;
    public int currentNodeIndex = 0;
    private Transform currentPatrolDestination;
    public float nodeCompleteDistance = 0.5f;

    [Header("Waiting")]
    public float t_currentWaitTimer = 0f;

    [Header("Investigate")]
    public float investigateDistance = 1f;
    public float timeBeforeLookAround = 3f;
    private float t_timeBeforeLookAround = 0f;

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

    [Header("Reactions")]
    public float reactionTime = 1f;
    private float t_reactionTime = 0f;
    private e_EnemyStates reactNextState;
    public float reactionCooldown = 4f;
    private float t_reactionCooldown = 0f;

    [Header("Flinch")]
    public float flinchTime = 0.2f;
    private float t_flinchTime = 0;

    [Header("Attack")]
    public float attackCooldown = 1f;
    private float t_attackCooldown = 0;
    public GameObject swordObject;
    public GameObject attackTrigger;
    private bool f_attackInit = false;
    public bool f_playerInAttackZone = false;


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
        animator = graphicsObject.GetComponent<Animator>();


        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

        fuckyou = new Vector3(facingDirection, 1, 1);

        patrolRoute = patrolRouteObject.GetComponent<PatrolRoute>();
        if (patrolRoute != null)
        {
            currentPatrolDestination = patrolRoute.nodes[currentNodeIndex];
        }

    }

    private void Update()
    {
        UpdateAnimator();

        if (graphicsObject.transform.localScale.x != facingDirection)
        {
            graphicsObject.transform.localScale = new Vector3(facingDirection, 1, 1);
            attackTrigger.transform.localPosition = new Vector3(facingDirection, 0, 0);
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
            case e_EnemyStates.reaction:
                ProcessReaction();
                break;
            case e_EnemyStates.damageFlinch:
                ProcessFlinch();
                break;
            case e_EnemyStates.attacking:
                ProcessAttack();
                break;
        }

        //if (awareScript.currentAwareness == AwarenessLevel.curious) currentState = e_EnemyStates.investigate;

        ApplyMovement();

        // update facing direction
        if (Mathf.Abs(movementVector.x) > 0.1)  SwitchFacing(Mathf.Sign(movementVector.x));

        fuckyou = new Vector3(facingDirection, 1, 1);


        if(t_flinchTime > 0 && currentState != e_EnemyStates.damageFlinch)
        {
            previousState = currentState;
            ChangeState(e_EnemyStates.damageFlinch);
        }

        if(f_playerInAttackZone && t_attackCooldown <= 0)
        {
            ChangeState(e_EnemyStates.attacking);
        }


        if (sightCone != null)  sightCone.transform.localPosition = sightConePosition;

        // manage timers
        if(t_swivelChangeTime > 0) t_swivelChangeTime -= Time.deltaTime;
        if (t_swivelStateTime > 0) t_swivelStateTime -= Time.deltaTime;
        if (t_swivelWalkTime > 0) t_swivelWalkTime -= Time.deltaTime;
        if (t_currentWaitTimer > 0) t_currentWaitTimer -= Time.deltaTime;
        if (t_reactionTime > 0) t_reactionTime -= Time.deltaTime;
        if (t_reactionCooldown > 0) t_reactionCooldown -= Time.deltaTime;
        if (t_timeBeforeLookAround > 0) t_timeBeforeLookAround -= Time.deltaTime;
        if (t_flinchTime > 0) t_flinchTime -= Time.deltaTime;
        if (t_attackCooldown > 0) t_attackCooldown -= Time.deltaTime;
        if (t_facingSwitchTimer > 0) t_facingSwitchTimer -= Time.deltaTime;
    }









    // Update Functions for States
    private void ProcessPatrolling()
    {
        float nodeDistance = (currentPatrolDestination.position - transform.position).magnitude;
        if (nodeDistance <= nodeCompleteDistance)
        {
            t_currentWaitTimer = patrolRoute.waitTimes[currentNodeIndex];
            SetNextNodeIndex();

            if (conditions.Contains(e_EnemyConditions.vigilant))
                ChangeState(e_EnemyStates.headSwivel);
            else
                ChangeState(e_EnemyStates.waiting);
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

        if (Mathf.Sign(targetLookPosition.x - transform.position.x) != facingDirection)
        {
            //facingDirection = Mathf.Sign(targetLookPosition.x - transform.position.x);
            SwitchFacing(Mathf.Sign(targetLookPosition.x - transform.position.x));
        }

        if (dist > investigateDistance)
        {
            pathfindTarget = awareScript.lastKnownPosition;

            PathFollow();
        }
        else inputVector = Vector3.zero;

        if (!awareScript.f_playerInSight && t_timeBeforeLookAround <= 0) 
        {
            ChangeState(e_EnemyStates.headSwivel);
        }

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

        if (Mathf.Sign(targetLookPosition.x - transform.position.x) != facingDirection)
        {
            //facingDirection = Mathf.Sign(targetLookPosition.x - transform.position.x);
            SwitchFacing(Mathf.Sign(targetLookPosition.x - transform.position.x));
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
            if (awareScript.currentAwareness == AwarenessLevel.unaware)
                ChangeState(e_EnemyStates.patrolling);
            else
                ChangeState(e_EnemyStates.investigate);
        }


    }

    private void ProcessReaction()
    {
        if (t_reactionTime <= 0)
            ChangeState(reactNextState);

        targetLookPosition = awareScript.lastKnownPosition;
        SightConeTrack();
    }

    private void ProcessFlinch()
    {
        if (t_flinchTime <= 0)
            ChangeState(previousState);
    }


    private void ProcessAttack()
    {
        if (!f_attackInit)
        {
            f_attackInit = true;
            swordObject.SetActive(true);
            //swordObject.transform.position = transform.position;
            swordObject.transform.localScale = new Vector3(facingDirection, 1, 1);
            swordObject.GetComponentInChildren<Animator>().SetTrigger("swing");
            swordObject.GetComponentInChildren<SwordScript>().animating = true;
            swordObject.transform.GetChild(0).transform.localPosition = new Vector3(0.8f, 0, 0);
            swordObject.transform.GetChild(0).GetComponent<DamageSource>().RefreshDamageSource();
            //inputVector.x = swingMoveSpeed * playerFacingVector.x;

            t_attackCooldown = attackCooldown;

        }

        if (!swordObject.GetComponentInChildren<SwordScript>().animating)
        {
            swordObject.SetActive(false);
            f_attackInit = false;
            ChangeState(e_EnemyStates.investigate);
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

    public void TriggerReaction(e_EnemyStates state)
    {
        reactNextState = state;
        t_reactionTime = reactionTime;
        
        
        if (t_reactionCooldown <= 0)
        {
            Debug.Log("Reaction!");
            ChangeState(e_EnemyStates.reaction);
        }
            
        else
            ChangeState(state);

        t_reactionCooldown = reactionCooldown;
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

    // triggered by awareness script when sight is lost
    public void PlayerSightLost()
    {
        if(t_timeBeforeLookAround <= 0)
            t_timeBeforeLookAround = timeBeforeLookAround;
    }

    // called by the awareness script when the state chanegs
    public void AwarenessChange(AwarenessLevel newState)
    {
        if (newState == AwarenessLevel.alert)
        {
            if (!conditions.Contains(e_EnemyConditions.vigilant))
                conditions.Add(e_EnemyConditions.vigilant);
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



    // manage animator variables
    private void UpdateAnimator()
    {

        if (Mathf.Abs(movementVector.x) <= 1f) animator.SetBool("moving", false);
        else animator.SetBool("moving", true);

        if ( awareScript.currentAwareness != AwarenessLevel.alert ) animator.SetBool("alert", false);
        else animator.SetBool("alert", true);

        if (currentState == e_EnemyStates.damageFlinch) animator.SetBool("flinch", true);
        else animator.SetBool("flinch", false);
    }



    private void SwitchFacing(float newFacing)
    {
        if(t_facingSwitchTimer <= 0)
        {
            facingDirection = newFacing;
            t_facingSwitchTimer = facingSwitchTimer;
        }
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile")
        {
            
            DamageSource dmg = collision.gameObject.GetComponent<DamageSource>();
            if (dmg != null)
            {
                if(dmg.applyToTags.Contains(gameObject.tag) && !dmg.hasHit)
                {
                    dmg.hasHit = true;
                    currentHP -= dmg.damageAmount;
                    // trigger flinch state
                    t_flinchTime = flinchTime;
                    Debug.Log("oof ouch");
                }
            }
        }
    }



    // called by attack trigger 
    public void AttackTriggerEnter()
    {
        f_playerInAttackZone = true;
    }
    public void AttackTriggerExit()
    {
        f_playerInAttackZone = false;
    }





}
