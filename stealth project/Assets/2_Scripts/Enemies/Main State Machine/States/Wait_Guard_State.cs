using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait_Guard_State : Guard_State
{

    private Patrol_Guard_State patrol;

    void Start()
    {
        base.OnStart();
        patrol = GetComponent<Patrol_Guard_State>();
    }


    public override void OnUpdate()
    {
        if (patrol.t_currentWaitTimer <= 0) sm.ChangeStateEnum(e_EnemyStates.patrolling);
        em.inputVector = Vector2.zero;


        if (em.GetMovementVector().magnitude == 0)
            // look in right direction for the patrol point
            em.facingDirection = patrol.patrolFacing;

        float angle = 190;
        if (em.facingDirection > 0) angle = 350;
        Vector3 lookTarget = Vector3.zero;



        lookTarget = sm.utils.GetVectorFromAngle(angle) * 2;
        lookTarget = transform.TransformPoint(lookTarget);

        sm.targetLookPosition = lookTarget;


        sm.SightConeTrack();
    }
}
