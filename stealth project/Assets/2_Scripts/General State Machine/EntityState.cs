using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState : MonoBehaviour
{

    //public EntityStateMachine sm;

    private void Start()
    {
        OnStart();
    }

    public virtual void OnStart()
    {
        //sm = GetComponent<EntityStateMachine>();
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }

}
