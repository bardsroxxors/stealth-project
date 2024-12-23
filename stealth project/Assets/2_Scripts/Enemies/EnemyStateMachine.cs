using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using static UnityEngine.GraphicsBuffer;
using System;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;
using System.Linq;
using static UnityEditor.PlayerSettings;
using UnityEditor;
using System.IO;


public enum e_EnemyStates
{
    patrolling, // walking to next patrol point
    waiting,    // waiting at patrpol point
    investigate,// walking to last known position
    scramble,   // running around to random points
    headSwivel, // looking around
    reaction,   // reacting to new information
    damageFlinch,// reacting to minor damage
    attacking,   // normal attack
    jump,
    fall,
    dead
}

public enum e_EnemyConditions
{
    vigilant,       // Looks around frequently, has been alerted
    blind,
    immobile,
    piqued,         // has become curious
    alerted,        // has become alert
    oblivious,      // has moved past hidden player
    bodySighted,    // has sighted a body
    chimes,         
    forceMine
}

public class EnemyStateMachine : MonoBehaviour
{

    public e_EnemyStates currentState = e_EnemyStates.patrolling;
    private e_EnemyStates previousState;
    public int currentHP = 10;
    public int maxHP = 10;
    //public GameObject graphicsObject;
    private Animator animator;
    public List<e_EnemyConditions> conditions = new List<e_EnemyConditions>();
    public List<float> conditionTimes = new List<float>();
    public GameObject noisePrefab;
    public GameObject passiveNoisePrefab;
    public Sound snd_footstep;
    public Sound soundSO;
    private e_EnemyStates queuedState = e_EnemyStates.investigate;
    public GameObject graphicsObject;





    public float damageKnowTime = 2f;
    private float t_damageKnowTime = 0;
    public bool f_koLocked = false;


    [Header("Patrolling")]
    public GameObject patrolRouteObject;
    public int currentNodeIndex = 0;
    private Vector2 currentPatrolDestination;
    public float nodeCompleteDistance = 0.5f;
    public float patrolSpeed = 5;
    public float pursueSpeed = 8;

    [Header("Waiting")]
    public float t_currentWaitTimer = 0f;

    [Header("Investigate")]
    public float investigateDistance = 1f;
    public float timeBeforeLookAround = 3f;
    private float t_timeBeforeLookAround = 0f;
    private int lastKnownDirection = 0;

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


    

    [Header("Sight Cone")]
    public GameObject sightCone;
    public Vector2 sightConePosition = Vector2.zero;
    public float coneTrackingSpeed = 1;
    public Vector3 targetLookPosition = Vector3.zero;
    public GameObject defaultLookTarget;
    

    [Header("Reactions")]
    public float reactionTime = 1f;
    private float t_reactionTime = 0f;
    private e_EnemyStates reactNextState;
    public float reactionCooldown = 4f;
    private float t_reactionCooldown = 0f;

    [Header("Flinch")]
    public float flinchTime = 0.2f;
    private float t_flinchTime = 0;
    public GameObject bloodPrefab;

    

    [Header("Attack")]
    public float attackCooldown = 1f;
    private float t_attackCooldown = 0;
    public float attackRange = 1.2f;
    public GameObject swordObject;
    public GameObject attackTrigger;
    private bool f_attackInit = false;
    // used by animator to trigger attack
    public bool f_triggerAttack = false;
    public bool f_playerInAttackZone = false;

    [Header("Scramble")]
    public float randomPointRadius = 5;
    public LayerMask navpointsMask;
    public LayerMask levelCollisionMask;
    private bool f_init_scramble = false;
    private Vector3 scramblePos = Vector3.zero;
    public float waitTime = 1f;
    private float t_waitTime = 0;
    public bool f_waiting = false;
    public float timeBeforeScramble = 2f;
    private float t_timeBeforeScramble = 0;
    private bool f_waitingToScramble = false;

    [Header("Blind Chase")]
    public float blindChaseDistance = 4;
    public bool f_blindChase = false;

    private bool f_fallInit = false;


    Rigidbody2D rb;
    private EnemyAwareness awareScript;
    private Utilities utils = new Utilities();
    public GameObject deadEnemy;
    float patrolFacing = -1f;




    private PatrolRoute patrolRoute;
    private bool boomerangBackwards = false;

    private EntityMovement ec_movement;
    private EntityPathfinding ec_pathing;



    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        awareScript = GetComponent<EnemyAwareness>();
        animator = graphicsObject.GetComponent<Animator>();
        ec_movement = GetComponent<EntityMovement>();
        ec_pathing = GetComponent<EntityPathfinding>();


        

        



        FindNearestPatrolRoute();

        patrolRoute = patrolRouteObject.GetComponent<PatrolRoute>();
        if (patrolRoute != null)
        {
            //currentPatrolDestination = patrolRouteObject.transform;
            currentPatrolDestination = (Vector3)patrolRoute.nodes[currentNodeIndex] + patrolRouteObject.transform.position;
            awareScript.lastKnownPosition = currentPatrolDestination;
        }

    }

    private void Update()
    {
        UpdateAnimator();

        
    }

    void FixedUpdate()
    { 
        

        if (currentHP <= 0 && currentState != e_EnemyStates.dead)
        {
            Die();
        }

        if(!f_koLocked)
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
            //case e_EnemyStates.jump:
                //ProcessJump();
                //break;
            case e_EnemyStates.fall:
                ProcessFall();
                break;
            case e_EnemyStates.scramble:
                ProcessScramble();
                break;
        }

        // check if we need to fall
        if (ec_movement.GetCollisionDirections().y != -1 && currentState != e_EnemyStates.jump)
        {
            ChangeState(e_EnemyStates.fall);
        }


        // the flinch state is triggered by setting its timer
        if (t_flinchTime > 0 && currentState != e_EnemyStates.damageFlinch)
        {
            ChangeState(e_EnemyStates.damageFlinch);
        }

        // attack is triggered right away, doesn't ask permission from current state, which is quite rude
        if(f_playerInAttackZone && t_attackCooldown <= 0 && awareScript.currentAwareness == AwarenessLevel.alert && awareScript.f_playerInSight
            && currentState != e_EnemyStates.jump
            && currentState != e_EnemyStates.dead)
        {
            ChangeState(e_EnemyStates.attacking);
        }

        if (conditions.Contains(e_EnemyConditions.immobile))
        {
            ec_movement.lockGravity = true;
            ec_movement.lockMovement = true;
        }
        else
        {
            ec_movement.lockGravity = false;
            ec_movement.lockMovement = false;
        }
            


            // check if we are freshly piqued
            if (awareScript.currentAwareness == AwarenessLevel.curious)
        {
            AddCondition(e_EnemyConditions.piqued);

        }
        // check if we are freshly piqued
        if (awareScript.currentAwareness == AwarenessLevel.alert)
        {
            AddCondition(e_EnemyConditions.alerted);
        }


        // turn off attack sprite when its done
        if (swordObject.active && !swordObject.GetComponentInChildren<SwordScript>().animating)
        {
            swordObject.SetActive(false);
            f_attackInit = false;
            f_triggerAttack = false;
            ChangeState(e_EnemyStates.investigate);
        }


        if (sightCone != null)  sightCone.transform.localPosition = sightConePosition;

        ManageTimers();
        
    }









    // Update Functions for States
    private void ProcessPatrolling()
    {

        if (conditions.Contains(e_EnemyConditions.immobile))
        {
            t_currentWaitTimer = 1f;
            ChangeState(e_EnemyStates.headSwivel);
        }
            

        float nodeDistance = ((Vector3)currentPatrolDestination - transform.position).magnitude;
        
        if (nodeDistance <= nodeCompleteDistance)
        {
            patrolFacing = patrolRoute.directions[currentNodeIndex];
            t_currentWaitTimer = patrolRoute.waitTimes[currentNodeIndex];
            SetNextNodeIndex();

            if (conditions.Contains(e_EnemyConditions.vigilant))
                ChangeState(e_EnemyStates.headSwivel);
            else
                ChangeState(e_EnemyStates.waiting);

        }

        // look in the direction we're walking
        float angle = 190;
        if (ec_movement.facingDirection > 0) angle = 350;
        Vector3 lookTarget = Vector3.zero;

        lookTarget = utils.GetVectorFromAngle(angle) * randomPointDistance;
        lookTarget = transform.TransformPoint(lookTarget);

        targetLookPosition = lookTarget;
        //targetLookPosition = transform.position + Vector3.Scale(defaultLookTarget.transform.localPosition, facingVector);
        SightConeTrack();


        ec_pathing.SetPathfindTarget( (Vector3)patrolRoute.nodes[currentNodeIndex] + patrolRouteObject.transform.position  );
        ec_movement.inputVector = ec_pathing.PathDirection() * patrolSpeed;

    }

    // Waiting at a patrol node
    private void ProcessWaiting()
    {
        if (t_currentWaitTimer <= 0) ChangeState(e_EnemyStates.patrolling);
        ec_movement.inputVector = Vector2.zero;


        if(ec_movement.GetMovementVector().magnitude == 0)
        // look in right direction for the patrol point
            ec_movement.facingDirection = patrolFacing;

        float angle = 190;
        if (ec_movement.facingDirection > 0) angle = 350;
        Vector3 lookTarget = Vector3.zero;

        

        lookTarget = utils.GetVectorFromAngle(angle) * randomPointDistance;
        lookTarget = transform.TransformPoint(lookTarget);

        targetLookPosition = lookTarget;
        

        SightConeTrack();
    }

    // Moving towards last known position
    private void ProcessInvestigate()
    {

        float dist = (awareScript.lastKnownPosition - transform.position).magnitude;

        targetLookPosition = awareScript.lastKnownPosition;
        SightConeTrack();

        if (t_damageKnowTime > 0)
            targetLookPosition = GameObject.Find("Player").transform.position;

        if (dist > investigateDistance)
        {
            f_waitingToScramble = false;
            if(ec_pathing.GetPathFindTarget() != awareScript.lastKnownPosition)
            {
                ec_pathing.SetPathfindTarget(awareScript.lastKnownPosition);
            }
            ec_movement.inputVector = ec_pathing.PathDirection() * pursueSpeed;

        }
        else
        {
            ec_movement.inputVector = Vector3.zero;
           
            if (queuedState == e_EnemyStates.scramble && !f_waitingToScramble)
            {
                f_waitingToScramble = true;
                t_timeBeforeScramble = timeBeforeScramble;
            }
        }
            

        if (Mathf.Sign(targetLookPosition.x - transform.position.x) != ec_movement.facingDirection)
        {
            ec_movement.SwitchFacing(Mathf.Sign(targetLookPosition.x - transform.position.x));
        }

        

        if (!awareScript.f_playerInSight && t_timeBeforeScramble <= 0 && f_waitingToScramble) 
        {
            //Debug.Log("Scramblin' time!");
            f_waitingToScramble = false;
            queuedState = e_EnemyStates.investigate;
            ChangeState(e_EnemyStates.scramble);
        }

    }

    // Lost sight of player and moving head around
    private void ProcessHeadSwivel()
    {
        if (!f_init_swivel)
        {
            f_init_swivel = true;
            t_swivelStateTime = swivelStateTime;
            t_swivelChangeTime = 0;
        }

        if (ec_movement.inputVector != Vector2.zero) ec_movement.inputVector = Vector2.zero;

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

        if (Mathf.Sign(targetLookPosition.x - transform.position.x) != ec_movement.facingDirection)
        {
            //facingDirection = Mathf.Sign(targetLookPosition.x - transform.position.x);
            ec_movement.SwitchFacing(Mathf.Sign(targetLookPosition.x - transform.position.x));
        }


        float dist = (lookTarget - transform.position).magnitude;
        SightConeTrack();

        ec_movement.inputVector = Vector3.zero;

        if (t_swivelStateTime <= 0)
        {
            f_init_swivel = false;
            if (awareScript.currentAwareness == AwarenessLevel.unaware)
                ChangeState(e_EnemyStates.patrolling);
            else
                ChangeState(e_EnemyStates.investigate);
        }


    }

    // Delay before taking action when player enters sight
    private void ProcessReaction()
    {
        if (t_reactionTime <= 0)
            ChangeState(reactNextState);

        targetLookPosition = awareScript.lastKnownPosition;
        SightConeTrack();
    }

    // Takes damage
    private void ProcessFlinch()
    {
        t_damageKnowTime = damageKnowTime;
        if (t_flinchTime <= 0)
        {
            PlayerSightGained(e_EnemyStates.investigate);
        }
        targetLookPosition = awareScript.lastKnownPosition;
        SightConeTrack();
    }




    // Falling
    private void ProcessFall()
    {
        float xMomentum = 0;
        if (f_fallInit)
        {
            xMomentum = ec_movement.inputVector.x;
            f_fallInit = true;
        }

        ec_movement.inputVector = new Vector2(xMomentum, 0);

        if(ec_movement.GetCollisionDirections().y == -1)
        {
            f_fallInit = false;
            ChangeState(previousState);
        }
    }


    // to attack, in update we get put into attack state
    // this makes animator go to windup animation
    // windup animation triggers bool to activate the rest of the stuff in attack state
    private void ProcessAttack()
    {

        targetLookPosition = awareScript.lastKnownPosition;
        SightConeTrack();

        if (!f_attackInit && f_triggerAttack)
        {
            f_attackInit = true;
            f_triggerAttack = false;

            swordObject.SetActive(true);
            //swordObject.transform.position = transform.position;
            swordObject.transform.localScale = new Vector3(ec_movement.facingDirection, 1, 1);
            swordObject.GetComponentInChildren<Animator>().SetTrigger("swing");
            swordObject.GetComponentInChildren<SwordScript>().animating = true;

            // so in here is where we can set the position and rotation of the sword object when the attack goes off
            // using the sightcone position I guess? That makes sense
            // right now we're using a vector to set the localposition of the swing 
            // so instead of setting it to that we need a normalised vector pointng in the right direction,
            //  and times that by the magnitude we want the range to be
            // then when we set the rotation we need to constrain it to (90, -90) facing right,
            // because the facing is determined by scale I'm pretty sure

            // utils.GetVectorFromAngle() to get our attack angle?

            Vector2 attackDirection = (targetLookPosition - transform.position).normalized;
            attackDirection.x = Mathf.Abs(attackDirection.x);
            float angle = utils.GetAngleFromVectorFloat(attackDirection);

            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            swordObject.transform.GetChild(0).transform.localPosition = attackDirection * attackRange;
            swordObject.transform.GetChild(0).transform.localRotation = targetRotation;

            swordObject.transform.GetChild(0).GetComponent<DamageSource>().RefreshDamageSource();
            //inputVector.x = swingMoveSpeed * playerFacingVector.x;

            t_attackCooldown = attackCooldown;

        }

        if (!swordObject.GetComponentInChildren<SwordScript>().animating && f_attackInit)
        {
            swordObject.SetActive(false);
            f_attackInit = false;
            f_triggerAttack = false;
            ChangeState(e_EnemyStates.investigate);
        }
    }


    // this is the stat where the enemy runs around at random
    /*
     maybe do it by taking a circle around the enemy, picking a random point in that circle,
    then translate that to the a walkable point below that point?

    easier way, get a random navpoint from in that circle
     */
    private void ProcessScramble()
    {
        if (!f_init_scramble)
        {
            scramblePos = GetRandomNavPoint().transform.position;
            f_init_scramble = true;
        }

        float dist = (scramblePos - transform.position).magnitude;

        targetLookPosition = scramblePos;
        SightConeTrack();

        if (dist > investigateDistance)
        {
            if (ec_pathing.GetPathFindTarget() != scramblePos)
            {
                ec_pathing.SetPathfindTarget( scramblePos);
            }

        }
        else
        {
            ec_movement.inputVector = Vector3.zero;
            if (!f_waiting)
            {
                f_waiting = true;
                t_waitTime = waitTime;
            }
            
        }

        if (f_waiting && t_waitTime <= 0)
        {
            f_waiting = false;
            scramblePos = GetRandomNavPoint().transform.position;
        }

        if (Mathf.Sign(targetLookPosition.x - transform.position.x) != ec_movement.facingDirection)
        {
            ec_movement.SwitchFacing(Mathf.Sign(targetLookPosition.x - transform.position.x));
        }


    }


    private void CheckForJump()
    {   /*
        // check if we need to jump
        float xDelta = Mathf.Abs(path.vectorPath[currentWaypoint].x - transform.position.x);
        float yDelta = path.vectorPath[currentWaypoint].y - transform.position.y;
        if (xDelta <= jumpMinDistance.x
            && currentState != e_EnemyStates.jump
            && collisionDirections.y == -1
            && yDelta >= jumpMinDistance.y)
        {
            if (!conditions.Contains(e_EnemyConditions.immobile))
                Jump();
        }*/
    }

    private void KOStart()
    {
        f_koLocked = true;
    }

    private void KOEnd()
    {
        f_koLocked = false;
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

        currentPatrolDestination = (Vector3)patrolRoute.nodes[currentNodeIndex] + patrolRouteObject.transform.position;
        ec_pathing.SetPathfindTarget(currentPatrolDestination);


        awareScript.lastKnownPosition = currentPatrolDestination;

    }



    public void ChangeState(e_EnemyStates state)
    {
        //if (currentState == e_EnemyStates.jump) f_jumpInit = false;

        if(currentState != e_EnemyStates.attacking || 
            (currentState == e_EnemyStates.attacking && f_attackInit == false))
        {
            if(currentState != e_EnemyStates.fall &&
                currentState != e_EnemyStates.jump)
            previousState = currentState;

            if (state == e_EnemyStates.patrolling ||
                state == e_EnemyStates.investigate ||
                state == e_EnemyStates.scramble)
            f_init_scramble = false;
            currentState = state;
        }

        
    }


    

    private void KunaiHit()
    {
            
        awareScript.lastKnownPosition = GameObject.FindWithTag("Player").transform.position;
        NoiseHeard(awareScript.lastKnownPosition, 0.3f);
        PlayerSightGained(e_EnemyStates.investigate);
        
    }

    public void PlayerSightGained(e_EnemyStates state)
    {
        f_blindChase = false;

        reactNextState = state;
        t_reactionTime = reactionTime;

        targetLookPosition = awareScript.lastKnownPosition;
        ec_pathing.SetPathfindTarget(awareScript.lastKnownPosition);


        lastKnownDirection = Math.Sign(targetLookPosition.x - transform.position.x);


        if (t_reactionCooldown <= 0 && currentState != e_EnemyStates.jump && currentState != e_EnemyStates.fall)
        {
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

        if (awareScript.currentAwareness != AwarenessLevel.unaware)
        {
            if (!f_blindChase)
            {
                f_blindChase = true;
                awareScript.lastKnownPosition = BlindPositionOffset(blindChaseDistance, awareScript.lastKnownPosition);
            }
            queuedState = e_EnemyStates.scramble;
        }
            
    }

    

    // called by the awareness script when the state chanegs
    public void AwarenessChange(AwarenessLevel newState)
    {
        if (newState == AwarenessLevel.alert)
        {
            if (!conditions.Contains(e_EnemyConditions.vigilant))
            {
                ApplyCondition(e_EnemyConditions.vigilant, -1f);
            }
                
            GameObject s = Instantiate(noisePrefab, transform.position, Quaternion.identity);
            s.SendMessage("SetProfile", soundSO);
        }

        else if (newState == AwarenessLevel.unaware) ChangeState(e_EnemyStates.patrolling);
    }

    public void BodySighted()
    {
        AddCondition(e_EnemyConditions.bodySighted);

        if(awareScript.currentAwareness != AwarenessLevel.alert)
        {
            awareScript.alertPercent = 1;
            PlayerSightGained(e_EnemyStates.scramble);
            queuedState = e_EnemyStates.investigate;
        }
        
    }
    
    public void ApplyCondition(e_EnemyConditions con, float time)
    {
        conditions.Add(con);
        conditionTimes.Add(time);
    }


    // manage animator variables
    private void UpdateAnimator()
    {
        if (Mathf.Abs(ec_movement.GetMovementVector().x) <= 1f) animator.SetBool("moving", false);
        else animator.SetBool("moving", true);

        if ( awareScript.currentAwareness != AwarenessLevel.alert ) animator.SetBool("alert", false);
        else animator.SetBool("alert", true);

        if (currentState == e_EnemyStates.damageFlinch) animator.SetBool("flinch", true);
        else animator.SetBool("flinch", false);

        if (currentState == e_EnemyStates.attacking) animator.SetBool("attack windup", true);
        else animator.SetBool("attack windup", false);

        if (currentState == e_EnemyStates.dead)
        {
            animator.SetBool("dead", true);
            animator.SetBool("attack windup", false);
            animator.SetBool("flinch", false);
            animator.SetBool("alert", false);
            animator.SetBool("moving", false);
        }
    }

    // use this to change facing, it uses a timer to prevent jettery behaviour
    

    private void ManageTimers()
    {
        if (t_swivelChangeTime > 0) t_swivelChangeTime -= Time.deltaTime;
        if (t_swivelStateTime > 0) t_swivelStateTime -= Time.deltaTime;
        if (t_swivelWalkTime > 0) t_swivelWalkTime -= Time.deltaTime;
        if (t_currentWaitTimer > 0) t_currentWaitTimer -= Time.deltaTime;
        if (t_reactionTime > 0) t_reactionTime -= Time.deltaTime;
        if (t_reactionCooldown > 0) t_reactionCooldown -= Time.deltaTime;
        if (t_timeBeforeLookAround > 0) t_timeBeforeLookAround -= Time.deltaTime;
        if (t_flinchTime > 0) t_flinchTime -= Time.deltaTime;
        if (t_attackCooldown > 0) t_attackCooldown -= Time.deltaTime;
        
        if (t_waitTime > 0) t_waitTime -= Time.deltaTime;
        if (t_timeBeforeScramble > 0) t_timeBeforeScramble -= Time.deltaTime;
        if (t_damageKnowTime > 0) t_damageKnowTime -= Time.deltaTime;


        for (int i = conditionTimes.Count - 1; i > -1; i--)
        {
            if (conditionTimes[i] != -1)
            {
                conditionTimes[i] -= Time.deltaTime;
            }
            if (conditionTimes[i] <= 0)
            {
                conditionTimes.RemoveAt(i);
                conditions.RemoveAt(i);
            }
        }
    }



    






    private GameObject GetRandomNavPoint()
    {
        Collider2D[] points = new Collider2D[10];

        points = Physics2D.OverlapCircleAll(transform.position, randomPointRadius, navpointsMask, -5, 5);

        if (points.Length > 0) 
        {
            return points[(int)Random.Range(0, points.Length - 1)].gameObject;
        }
        else return null;
        
    }

    // used to offset last known position when sight is lost
    // allows a short blind chase after sight is lost
    private Vector3 BlindPositionOffset(float distance, Vector2 position)
    {
        int dir = Math.Sign(awareScript.lastKnownPosition.x - transform.position.x);

        // Do a raycast to see if we hit a wall
        // if we do then return a slight offset from the wall

        // else offset by the full distance

        RaycastHit2D ray = Physics2D.Raycast(position, Vector2.right* dir, distance, levelCollisionMask);
        if (ray)
            return ray.point;// + (Vector2.right * -dir * 2);

        return (Vector3)(position + (Vector2.right * dir * distance));

    }

    public bool AddCondition(e_EnemyConditions con)
    {
        if (conditions.Contains(con)) return false;

        else
        {
            conditions.Add(con);
            switch (con)
            {
                case e_EnemyConditions.piqued:
                    this.SendMessage("AddBounty", 1);
                    break;
                case e_EnemyConditions.oblivious:
                    this.SendMessage("AddBounty", 1);
                    break;
                case e_EnemyConditions.alerted:
                    this.SendMessage("SubtractBounty", 1);
                    break;
                case e_EnemyConditions.bodySighted:
                    this.SendMessage("AddBounty", 2);
                    break;
                case e_EnemyConditions.chimes:
                    this.SendMessage("AddBounty", 2);
                    break;
                case e_EnemyConditions.forceMine:
                    this.SendMessage("AddBounty", 1);
                    break;
                case e_EnemyConditions.immobile:
                    this.SendMessage("AddBounty", 1);
                    break;
            }
            return true;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerProjectile" && currentState != e_EnemyStates.dead)
        {
            // i wanna make it so damage doesn't happen if the player is in their cone of vision
            // get sightcone angle, that's the z rotation
            // get angle to the player

            DamageSource dmg = collision.gameObject.GetComponent<DamageSource>();
            if (dmg != null)
            {
                if(dmg.applyToTags.Contains(gameObject.tag) && !dmg.hasHit)
                {
                    dmg.hasHit = true;
                    currentHP -= dmg.damageAmount;
                    // trigger flinch state
                    t_flinchTime = flinchTime;
                    awareScript.alertPercent += 0.3f;
                    Instantiate(bloodPrefab, transform.position, Quaternion.identity);
                    awareScript.lastKnownPosition = collision.transform.position;
                    if (currentHP <= 0) Die();
                }
            }
        }

        
    }

    

    public void ObliviousIdiot()
    {
        AddCondition(e_EnemyConditions.oblivious);

    }

    public void NoiseHeard(Vector3 position, float awareIncrease) 
    {
        //EditorApplication.isPaused = true;
        awareScript.alertPercent += awareIncrease;

        if (awareScript.currentAwareness == AwarenessLevel.unaware)
            PlayerSightGained(e_EnemyStates.investigate);
    }

    public void NoiseHeard(Vector3 position, float awareIncrease, e_EnemyConditions cond)
    {
        //EditorApplication.isPaused = true;
        awareScript.alertPercent += awareIncrease;

        AddCondition(cond);

        if (awareScript.currentAwareness == AwarenessLevel.unaware)
            PlayerSightGained(e_EnemyStates.investigate);
    }

    public void TriggerFootstep()
    {
        GameObject noise = Instantiate(passiveNoisePrefab, transform.position, Quaternion.identity);
        noise.SendMessage("SetProfile", snd_footstep);
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
    // called by sprite object to trigger attack
    public void TriggerAttack()
    {
        f_triggerAttack = true;
    }

 

    private void Die()
    {
        sightCone.SetActive(false);
        //ChangeState(e_EnemyStates.dead);
        awareScript.Die();
        this.SendMessage("GrantPoints");
        //UpdateAnimator();
        Vector3 offset = new Vector3(0, 0.5f, 0);
        GameObject dead = Instantiate(deadEnemy, transform.position + offset, Quaternion.identity);
        if(ec_movement.facingDirection == -1)
            dead.GetComponent<SpriteRenderer>().flipX = true;
        Destroy(this.gameObject);
    }

    private void StealthKilled()
    {
        //if (awareScript.currentAwareness != AwarenessLevel.alert)
        Die();
    }

    public float GetFacingDirection()
    {
        return ec_movement.facingDirection;
    }

    void FindNearestPatrolRoute()
    {
        Debug.Log("Finding nearest route");
        GameObject[] routes = GameObject.FindGameObjectsWithTag("PatrolRoute");
        float minDist = 1000f;
        GameObject nearest = null;

        if(routes != null)
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


    private void OnDrawGizmos()
    {
        Handles.color = UnityEngine.Color.red;
        Handles.DrawWireCube(targetLookPosition, new Vector3(0.25f, 0.25f, 0.25f));

        Handles.color = UnityEngine.Color.blue;
        Handles.DrawWireCube(currentPatrolDestination, new Vector3(0.4f, 0.4f, 0.4f));
        if(patrolRouteObject != null)
            Handles.DrawWireDisc(patrolRouteObject.transform.position, Vector3.forward, 0.4f);

        /*
        if (path != null && currentWaypoint < path.vectorPath.Count - 1)
        {
            Handles.color = UnityEngine.Color.green;
            if (path != null && path.vectorPath.Count > 0 && Application.isPlaying)
                Handles.DrawWireCube(path.vectorPath[currentWaypoint], new Vector3(0.25f, 0.25f, 0.25f));

        }*/

        
    }

}
