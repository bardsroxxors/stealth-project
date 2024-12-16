using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard_State : EntityState
{
    [HideInInspector]
    public Guard_StateMachine sm;
    [HideInInspector]
    public EntityMovement em;
    [HideInInspector]
    public Collider2D collider;
    [HideInInspector]
    public ConditionManager cond;
    [HideInInspector]
    public EnemyAwareness awareScript;
    [HideInInspector]
    public EntityPathfinding pathing;


    // Start is called before the first frame update
    void Start()
    {
        base.OnStart();
        sm = GetComponent<Guard_StateMachine>();
        em = GetComponent<EntityMovement>();
        collider = GetComponent<Collider2D>();
        cond = GetComponent<ConditionManager>();
        awareScript = GetComponent<EnemyAwareness>();
        pathing = GetComponent<EntityPathfinding>();
    }


}
