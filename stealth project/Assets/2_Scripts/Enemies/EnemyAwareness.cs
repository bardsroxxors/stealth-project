using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AwarenessLevel
{
    unaware,
    curious,
    alert,
    searching
}
public class EnemyAwareness : MonoBehaviour
{

    public AwarenessLevel currentAwareness = AwarenessLevel.unaware;
    public float alertPercent = 0f;
    public bool f_playerInSight = false;
    public GameObject playerObject;
    public Vector3 lastKnownPosition;
    public GameObject yellowIndicator;
    public GameObject redIndicator;
    public GameObject sightCone;
    public LayerMask layerMask;

    private SpriteRenderer yellowSprite;
    private SpriteRenderer redSprite;
    private EnemyStateMachine mainScript;

    [Header("Awareness Numbers")]
    public float sightAwareIncreaseSpeedMin = 0.3f;
    public float sightAwareIncreaseSpeedMax = 0.9f;
    //public float soundAwareIncrease = 0.5f;
    public float awarenessDecaySpeed = 0.2f;
    public float alertDecaySpeed = 0.15f;
    public float alertDecayDelay = 0.5f;
    private float t_alertDecayDelay = 0f;
    public float minAwareIncreaseDistance = 10f;
    public float vigilanceMultiplier = 2f;

    private bool previousFrame_playerInSight = false;



    void Start()
    {
        playerObject = GameObject.Find("Player");

        if (yellowIndicator != null) yellowSprite = yellowIndicator.gameObject.GetComponent<SpriteRenderer>();

        if (redIndicator != null) redSprite = redIndicator.gameObject.GetComponent<SpriteRenderer>();

        mainScript = GetComponent<EnemyStateMachine>();
    }


    void FixedUpdate()
    {
        //Debug.DrawRay(sightCone.transform.position, (playerObject.transform.position - sightCone.transform.position));
        switch (currentAwareness)
        {
            case AwarenessLevel.unaware:
                ProcessUnaware();
                break;
            case AwarenessLevel.curious:
                ProcessCurious();
                break;
            case AwarenessLevel.alert:
                ProcessAlert();
                break;
            case AwarenessLevel.searching:
                ProcessSearching();
                break;
        }
        /*
        if (CheckForLitPlayer())
            Debug.Log("Lit player");
            PlayerInSight();*/

        if (t_alertDecayDelay > 0) t_alertDecayDelay -= Time.deltaTime;

        alertPercent = Mathf.Clamp(alertPercent, 0, 1);


        if(f_playerInSight && !previousFrame_playerInSight)
        {
            PlayerEnteredSight();
        }
        else if(!f_playerInSight && previousFrame_playerInSight)
        {
            mainScript.PlayerSightLost();
        }


        previousFrame_playerInSight = f_playerInSight;
    }



    private void ProcessUnaware()
    {
        if (f_playerInSight)
        {
            alertPercent = alertPercent + GetIncreaseSpeed() * Time.deltaTime;
            t_alertDecayDelay = alertDecayDelay;
        }

        if (alertPercent > 0f)
        {
            ChangeState(AwarenessLevel.curious);
        }
    }

    private void ProcessCurious()
    {

        yellowIndicator.SetActive(true);
        yellowSprite.color = new Color(1f,1f,1f, alertPercent);


        if(alertPercent == 1)
        {
            yellowIndicator.SetActive(false);
            ChangeState(AwarenessLevel.alert);
        }

        if(f_playerInSight && playerObject != null)
        {
            lastKnownPosition = playerObject.transform.position;
            alertPercent = alertPercent + GetIncreaseSpeed() * Time.deltaTime;
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
        }
    }

    private void ProcessAlert()
    {
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
        }
    }

    private void ProcessSearching()
    {

    }


    private void ChangeState(AwarenessLevel newState)
    {
        if(currentAwareness != newState)
            mainScript.AwarenessChange(newState);
        currentAwareness = newState;
    }

    // get the awareness increase speed based on distance
    private float GetIncreaseSpeed()
    {
        float multiplier = 1f;
        if (mainScript.conditions.Contains(e_EnemyConditions.vigilant)) multiplier = vigilanceMultiplier;

        float dist = (transform.position - lastKnownPosition).magnitude;
        float num;
        if(dist < minAwareIncreaseDistance)
        {
            num = Mathf.Lerp(sightAwareIncreaseSpeedMax, sightAwareIncreaseSpeedMin, (dist / minAwareIncreaseDistance)) * multiplier;
        }
        else
        {
            num = sightAwareIncreaseSpeedMin * multiplier;
        }


        return num;
    }


    // called when player has entered the sight cone
    public void PlayerInSight()
    {
        f_playerInSight = GetRayToPlayer();
    }

    // called when the player has left the sight cone
    public void PlayerSightLost()
    {
        f_playerInSight = false;
    }

    // called when f_playerInSight has changed to true
    public void PlayerEnteredSight()
    {
        mainScript.TriggerReaction(e_EnemyStates.investigate);
    }

    // called when the entity is inside a noise trigger
    public void NoiseDetected()
    {
        //alertPercent += soundAwareIncrease;
    }



    // check if we have direct LOS to player
    private bool GetRayToPlayer()
    {
        Vector3 dir = playerObject.transform.position - sightCone.transform.position;
        RaycastHit2D ray = Physics2D.Raycast(sightCone.transform.position, dir * 50, 50, layerMask);

        

        if (ray && ray.collider.gameObject.tag == "Player") return true;
        else return false;

    }


}
