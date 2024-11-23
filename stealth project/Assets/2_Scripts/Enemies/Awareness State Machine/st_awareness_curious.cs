using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class st_awareness_curious : EntityState
{
    SM_Awareness sm;


    public override void OnStart()
    {
        sm = GetComponent<SM_Awareness>();
    }

    public override void OnUpdate()
    {
        
        /*
        sm.yellowIndicator.SetActive(true);
        sm.yellowSprite.color = new Color(1f, 1f, 1f, sm.alertPercent);


        if (sm.alertPercent == 1)
        {
            sm.yellowIndicator.SetActive(false);
            sm.ChangeState(sm.st_alert);
        }

        if (sm.f_playerInSight && sm.playerObject != null && !mainScript.f_koLocked)
        {
            // todo what is this? Can we turn it into a function in the sm?
            sm.lastKnownPosition = sm.playerObject.transform.position;
            sm.alertPercent = sm.alertPercent + sm.GetIncreaseSpeed() * Time.deltaTime;
            t_alertDecayDelay = alertDecayDelay;
            if (!GetRayToPlayer()) f_playerInSight = false;
        }


        else if (alertPercent > 0f && t_alertDecayDelay <= 0)
        {
            alertPercent -= awarenessDecaySpeed * Time.deltaTime;

            if (alertPercent < 0.1f)
            {
                alertPercent = 0f;
                ChangeState(AwarenessLevel.unaware);
                yellowSprite.color = new Color(1f, 1f, 1f, 0f);
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
