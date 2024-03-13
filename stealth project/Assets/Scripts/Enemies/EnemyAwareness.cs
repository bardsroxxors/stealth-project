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
    public bool playerInSight = false;
    public GameObject playerObject;
    public Vector3 lastKnownPosition;
    public GameObject yellowIndicator;
    public GameObject redIndicator;

    private SpriteRenderer yellowSprite;
    private SpriteRenderer redSprite;
    private EnemyStateMachine mainScript;

    [Header("Awareness Numbers")]
    public float sightAwareIncreaseSpeed = 0.3f;
    public float soundAwareIncrease = 0.5f;
    public float awarenessDecaySpeed = 0.2f;
    public float alertDecaySpeed = 0.15f;
    public float alertDecayDelay = 0.5f;
    private float t_alertDecayDelay = 0f;

    //[Header("Unaware")]

    

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player");

        if (yellowIndicator != null) yellowSprite = yellowIndicator.gameObject.GetComponent<SpriteRenderer>();

        if (redIndicator != null) redSprite = redIndicator.gameObject.GetComponent<SpriteRenderer>();

        mainScript = GetComponent<EnemyStateMachine>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

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


        if (t_alertDecayDelay > 0) t_alertDecayDelay -= Time.deltaTime;

        alertPercent = Mathf.Clamp(alertPercent, 0, 1);
    }



    private void ProcessUnaware()
    {
        if (playerInSight)
        {
            alertPercent = alertPercent + sightAwareIncreaseSpeed * Time.deltaTime;
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

        if(playerInSight && playerObject != null)
        {
            lastKnownPosition = playerObject.transform.position;
            alertPercent = alertPercent + sightAwareIncreaseSpeed * Time.deltaTime;
            t_alertDecayDelay = alertDecayDelay;
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

        if (playerInSight && playerObject != null)
        {
            lastKnownPosition = playerObject.transform.position;
            alertPercent = alertPercent + sightAwareIncreaseSpeed * Time.deltaTime;
            t_alertDecayDelay = alertDecayDelay;
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
        currentAwareness = newState;
        mainScript.AwarenessChange(newState);
    }


    // called when player has entered the sight cones
    public void PlayerInSight()
    {
        playerInSight = true;
    }

    // called when the player has left the sight cones
    public void PlayerSightLost()
    {
        playerInSight = false;
    }

    // called when the entity is inside a noise trigger
    public void NoiseDetected()
    {
        alertPercent += soundAwareIncrease;
    }
}
