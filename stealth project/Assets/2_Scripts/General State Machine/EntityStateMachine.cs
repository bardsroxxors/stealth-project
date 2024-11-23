using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateMachine : MonoBehaviour
{

    public List<EntityState> states = new List<EntityState>();
    public EntityState currentState;


    void Start()
    {
        
    }


    void FixedUpdate()
    {
        currentState.OnUpdate();
    }

    public virtual bool ChangeState(EntityState nextState)
    {
        if ((states.Contains(nextState)))
        {
            currentState.OnExit();
            currentState = nextState;
            currentState.OnEnter();
            return true;
        }
        else return false;
        
    }
}
