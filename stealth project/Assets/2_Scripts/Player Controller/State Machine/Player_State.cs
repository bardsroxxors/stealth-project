using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State : EntityState
{
    [HideInInspector]
    public Player_StateMachine psm;
    [HideInInspector]
    public EntityMovement em;
    [HideInInspector]
    public Collider2D collider;

    private void Start()
    {
        OnStart();
    }
    public override void OnStart()
    {
        base.OnStart();
        psm = GetComponent<Player_StateMachine>();
        em = GetComponent<EntityMovement>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
