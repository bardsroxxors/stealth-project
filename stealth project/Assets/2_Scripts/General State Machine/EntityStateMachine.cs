using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateMachine : MonoBehaviour
{

    //public List<EntityState> states = new List<EntityState>();
    public EntityState currentState;




    void FixedUpdate()
    {

        if(currentState != null)
            currentState.OnUpdate();
    }

    public virtual bool ChangeState(EntityState nextState)
    {
        if(currentState != null)
            currentState.OnExit();
        currentState = nextState;
        currentState.OnEnter();
        return true;

        
    }
}
