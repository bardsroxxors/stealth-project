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

    [Header("Awareness Numbers")]
    public float sightAwareIncreaseSpeed = 0.3f;
    public float soundAwareIncrease = 0.5f;
    public float awarenessDecaySpeed = 0.2f;

    //[Header("Unaware")]

    

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindWithTag("Player");
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





        if (playerInSight)
        {
            alertPercent = alertPercent + sightAwareIncreaseSpeed * Time.deltaTime;
        }

        else if (alertPercent > 0f)
        {
            alertPercent -= awarenessDecaySpeed * Time.deltaTime;
        }


        alertPercent = Mathf.Clamp(alertPercent, 0, 1);
    }



    private void ProcessUnaware()
    {
        if(alertPercent > 0f)
        {
            currentAwareness = AwarenessLevel.curious;
        }
    }

    private void ProcessCurious()
    {
        if(playerInSight && playerObject != null)
        {
            lastKnownPosition = playerObject.transform.position;
        }
    }

    private void ProcessAlert()
    {

    }

    private void ProcessSearching()
    {

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
