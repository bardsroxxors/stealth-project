using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class st_awareness_unaware : EntityState
{
    SM_Awareness sm;


    public override void OnUpdate()
    {
        /*
        if (f_playerInSight && !mainScript.f_koLocked)
        {
            alertPercent = alertPercent + GetIncreaseSpeed() * Time.deltaTime;
            t_alertDecayDelay = alertDecayDelay;
        }

        if (alertPercent > 0f)
        {
            ChangeState(AwarenessLevel.curious);
        }*/
    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }


}
