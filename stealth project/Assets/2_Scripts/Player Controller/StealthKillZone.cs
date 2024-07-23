using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthKillZone : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    private PlayerController pc;
    public GameObject currentTarget;


    void Start()
    {
        pc = transform.parent.GetComponent<PlayerController>();
    }

    private void Update()
    {
        transform.localPosition = Vector3.zero;

        foreach (GameObject target in targets)
        {
            if(//target.GetComponent<EnemyAwareness>().currentAwareness == AwarenessLevel.alert ||
                target.GetComponent<EnemyStateMachine>().currentState == e_EnemyStates.dead)
            {
                targets.Remove(target);
                currentTarget = GetKillTarget();
                pc.SendMessage("KOTarget");
            }
            currentTarget = GetKillTarget();
            pc.SendMessage("KOTarget");

        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the object is not already in the list
        if (!targets.Contains(collision.gameObject))
        {
            EnemyAwareness a = collision.gameObject.GetComponent<EnemyAwareness>();
            EnemyStateMachine s = collision.gameObject.GetComponent<EnemyStateMachine>();
            if (a != null && s != null)
            {
                if (//a.currentAwareness != AwarenessLevel.alert &&
                    s.currentState != e_EnemyStates.dead)
                {
                    //add the object to the list
                    targets.Add(collision.gameObject);
                    currentTarget = GetKillTarget();
                    pc.SendMessage("KOTarget");
                }
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if the object is in the list
        if (targets.Contains(collision.gameObject))
        {
            //remove it from the list
            targets.Remove(collision.gameObject);
            currentTarget = GetKillTarget();
            pc.SendMessage("KOTarget");
        }
    }

    GameObject GetKillTarget()
    {
        GameObject final = null;

        //killzoneObject.transform.localPosition = Vector3.zero;
        float shortestDistance = 1000f;

        if (targets.Count == 0) return null;

        foreach (GameObject target in targets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                final = target;

            }
        }


        return final;
    }
}
