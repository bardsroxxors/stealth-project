using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class st_awareness_alert : EntityState
{
    SM_Awareness sm;


    public override void OnUpdate()
    {/*
        redIndicator.SetActive(true);

        if (f_playerInSight && playerObject != null)
        {
            lastKnownPosition = playerObject.transform.position;
            alertPercent = alertPercent + GetIncreaseSpeed() * Time.deltaTime;
            t_alertDecayDelay = alertDecayDelay;
            if (!GetRayToPlayer()) f_playerInSight = false;
        }


        else if (alertPercent > 0f && t_alertDecayDelay <= 0)
        {
            alertPercent -= alertDecaySpeed * Time.deltaTime;

            if (alertPercent < 0.1f)
            {
                alertPercent = 0f;
                ChangeState(AwarenessLevel.unaware);
                redIndicator.SetActive(false);
            }
        }*/
    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }


}
