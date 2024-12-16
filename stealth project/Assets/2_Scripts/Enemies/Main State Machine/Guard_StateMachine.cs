using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

[RequireComponent(typeof(Patrol_Guard_State))]
[RequireComponent(typeof(Wait_Guard_State))]
[RequireComponent(typeof(ConditionManager))]
[RequireComponent(typeof(EntityMovement))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyAwareness))]
[RequireComponent(typeof(EntityPathfinding))]
public class Guard_StateMachine : EntityStateMachine
{

    public e_EnemyStates e_currentState = e_EnemyStates.patrolling;
    //private Guard_State currentState;
    public e_EnemyStates e_previousState;
    private e_EnemyStates queuedState = e_EnemyStates.investigate;
    public int currentHP = 10;
    public int maxHP = 10;

    // ####### Component References ########
    private EntityMovement em;
    private EntityPathfinding pathing;
    private Animator animator;
    private Patrol_Guard_State patrol;
    private EnemyAwareness awareScript;
    public Utilities utils = new Utilities();
    private ConditionManager cond;
    // #######       -------        ########

    [Header("Sound References")]
    public GameObject noisePrefab;
    public GameObject passiveNoisePrefab;
    public Sound snd_footstep;
    public Sound soundSO;

    [Header("Sight Cone")]
    public GameObject sightCone;
    public Vector2 sightConePosition = Vector2.zero;
    public float coneTrackingSpeed = 1;
    public Vector3 targetLookPosition = Vector3.zero;
    public GameObject defaultLookTarget;


    // Start is called before the first frame update
    void Start()
    {
        em = GetComponent<EntityMovement>();
        animator = em.graphicsObject.GetComponent<Animator>();
        patrol = GetComponent<Patrol_Guard_State>();
        awareScript = GetComponent<EnemyAwareness>();
        cond = GetComponent<ConditionManager>();
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (currentHP <= 0 && e_currentState != e_EnemyStates.dead)
        {
            Die();
        }

        // check if we need to fall
        if (em.GetCollisionDirections().y != -1 && e_currentState != e_EnemyStates.jump)
        {
            ChangeStateEnum(e_EnemyStates.fall);
        }

        if (currentState != null)
            currentState.OnUpdate();

        


        if (cond.conditions.Contains(e_EnemyConditions.immobile))
        {
            em.lockGravity = true;
            em.lockMovement = true;
        }
        else
        {
            em.lockGravity = false;
            em.lockMovement = false;
        }

        if (sightCone != null) sightCone.transform.localPosition = sightConePosition;
        ManageTimers();
    }

    private void ManageTimers()
    {

    }

    public void ChangeStateEnum(e_EnemyStates state)
    {
        Guard_State s = GetComponentState(state);
        if (s != null)
        {
            
            if (ChangeState(s))
            {
                e_previousState = e_currentState;
                e_currentState = state;
            }
                
        }
    }

    public Guard_State GetComponentState(e_EnemyStates state)
    {
        Guard_State s = null;
        switch (state)
        {
            case e_EnemyStates.patrolling:
                s = GetComponent<Patrol_Guard_State>();
                break;
            case e_EnemyStates.waiting:
                s = GetComponent<Wait_Guard_State>();
                break;
        }

        return s;
    }

    // move sight cone towards target
    public void SightConeTrack()
    {
        if (targetLookPosition != null)
        {
            Vector2 direction = targetLookPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            sightCone.transform.rotation = Quaternion.Slerp(sightCone.transform.rotation, targetRotation, coneTrackingSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimator()
    {

    }

    void Die()
    {

    }

}
