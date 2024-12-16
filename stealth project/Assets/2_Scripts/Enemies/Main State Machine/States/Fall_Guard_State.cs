using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall_Guard_State : Guard_State
{


    float xMomentum = 0;

    void Start()
    {
        base.OnStart();
    }


    public override void OnStart()
    {
        xMomentum = em.inputVector.x;
    }

    public override void OnUpdate()
    {
        

        em.inputVector = new Vector2(xMomentum, 0);

        if (em.GetCollisionDirections().y == -1)
        {
            sm.ChangeStateEnum(sm.e_previousState);
        }
    }
}
