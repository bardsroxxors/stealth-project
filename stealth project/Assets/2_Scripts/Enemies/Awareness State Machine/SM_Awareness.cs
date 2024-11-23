using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(st_awareness_alert))]
[RequireComponent(typeof(st_awareness_curious))]
[RequireComponent(typeof(st_awareness_unaware))]

public class SM_Awareness : EntityStateMachine
{

    public float alertPercent = 0f;
    [HideInInspector]
    public bool f_playerInSight = false;
    [HideInInspector]
    public GameObject playerObject;
    [HideInInspector]
    public Vector3 lastKnownPosition;
    public GameObject yellowIndicator;
    public GameObject redIndicator;
    public GameObject sightCone;
    public LayerMask layerMask;

    private SpriteRenderer yellowSprite;
    private SpriteRenderer redSprite;
    private EnemyStateMachine mainScript;

    [Header("Awareness Numbers")]
    public float sightAwareIncreaseSpeedMin = 0.5f;
    public float sightAwareIncreaseSpeedMax = 2.5f;
    //public float soundAwareIncrease = 0.5f;
    public float awarenessDecaySpeed = 0.05f;
    public float alertDecaySpeed = 0.05f;
    public float alertDecayDelay = 3f;
    private float t_alertDecayDelay = 0f;
    public float minAwareIncreaseDistance = 5f;
    public float vigilanceMultiplier = 2f;

    private bool previousFrame_playerInSight = false;


    [HideInInspector]
    public st_awareness_alert st_alert;
    public st_awareness_curious st_curious;
    public st_awareness_unaware st_unaware;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player");

        if (yellowIndicator != null) yellowSprite = yellowIndicator.gameObject.GetComponent<SpriteRenderer>();

        if (redIndicator != null) redSprite = redIndicator.gameObject.GetComponent<SpriteRenderer>();

        //todo mainScript = transform.parent.GetComponent<EnemyStateMachine>();
        st_alert = GetComponent<st_awareness_alert>();
        st_curious = GetComponent<st_awareness_curious>();
        st_unaware = GetComponent<st_awareness_unaware>();
    }


    void FixedUpdate()
    {
        if (t_alertDecayDelay > 0) t_alertDecayDelay -= Time.deltaTime;

        alertPercent = Mathf.Clamp(alertPercent, 0, 1);


        if (f_playerInSight && !previousFrame_playerInSight)
        {
            //todo PlayerEnteredSight();
        }
        else if (!f_playerInSight && previousFrame_playerInSight)
        {
            mainScript.PlayerSightLost();
        }


        previousFrame_playerInSight = f_playerInSight;
    }

    public override bool ChangeState(EntityState nextState)
    {
        bool b = base.ChangeState(nextState);
        return b;
        /*
        if(b)
            mainScript.AwarenessChange(nextState);
        return b;*/
    }
}
