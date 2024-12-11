using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttack_Player_State : Player_State
{

    [Header("Sword Swing")]
    public GameObject swordObject;
    public float attackCooldownTime = 0.5f;
    [HideInInspector]
    public float t_attackCooldown = 0;
    //public float attackLengthTime = 0.5f;

    //private float t_attackLength = 0;

    [Header("Dash Attack")]
    public bool f_canDashAttack = true;
    public float dashLockTime = 0.5f;
    public float dashSpeed = 10f;
    public float dashAttackRange = 1f;
    private float t_dashLockTime = 0;

    private Freemove_Player_State freeMove;
    private Vector2 attackDirection;


    // Start is called before the first frame update
    public override void OnStart()
    {
        base.OnStart();
        freeMove = GetComponent<Freemove_Player_State>();
    }


    public override void OnEnter()
    {
        base.OnEnter();

        attackDirection = (Vector3)psm.GetVectorToMouse();
        
        psm.playerFacingVector = new Vector2(Mathf.Sign(attackDirection.x), 0);

        swordObject.SetActive(true);
        swordObject.transform.GetChild(0).transform.gameObject.SetActive(true);
        swordObject.transform.GetChild(1).transform.gameObject.SetActive(false);

        swordObject.transform.position = transform.position;
        swordObject.transform.localScale = new Vector3(psm.playerFacingVector.x, 1, 1);
        swordObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("swing");
        swordObject.transform.GetChild(0).GetComponent<SwordScript>().animating = true;


        Vector2 abs = attackDirection;
        abs.x = Mathf.Abs(abs.x);

        float angle = psm.utils.GetAngleFromVectorFloat(abs);

        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        swordObject.transform.GetChild(0).transform.localPosition = abs * dashAttackRange;
        swordObject.transform.GetChild(0).transform.localRotation = targetRotation;


        //swordObject.transform.GetChild(0).transform.localPosition = new Vector3(1f, 0, 0);
        swordObject.transform.GetChild(0).GetComponent<DamageSource>().RefreshDamageSource();

        //inputVector.x = swingMoveSpeed * playerFacingVector.x;

        //animator.Play("attack", 0);
        psm.PlayAnimation("attack", 0);



        //t_attackLength = attackLengthTime;
        t_attackCooldown = attackCooldownTime;
        //animator.SetTrigger("attack trigger");

        t_dashLockTime = dashLockTime;

    }


    // Update is called once per frame
    public override void OnUpdate()
    {

        //if(collisionDirections.y != -1 && moveStickVector.magnitude >= 0.25)
        if (psm.moveStickVector.magnitude >= 0.25)
            em.inputVector.x = psm.moveStickVector.normalized.x * freeMove.moveSpeed;


        em.SetGravityVector(dashSpeed * attackDirection);
        Debug.Log(attackDirection);
        em.inputVector = Vector2.zero;

        if (!swordObject.transform.GetChild(0).GetComponent<SwordScript>().animating)
            swordObject.SetActive(false);



        t_dashLockTime -= Time.deltaTime;



        if (t_dashLockTime <= 0)
        {
            swordObject.SetActive(false);
            //if (moveStickVector.magnitude > 0.1f ||
            //        animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "attack")
            psm.ChangeStateEnum(e_PlayerControllerStates.FreeMove);

        }
    }

}
