using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthKillZone : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the object is not already in the list
        if (!targets.Contains(collision.gameObject))
        {
            //add the object to the list
            targets.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if the object is in the list
        if (targets.Contains(collision.gameObject))
        {
            //remove it from the list
            targets.Remove(collision.gameObject);
        }
    }
}
