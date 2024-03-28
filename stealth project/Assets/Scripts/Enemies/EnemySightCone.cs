using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightCone : MonoBehaviour
{
    private GameObject parent;

    void Start()
    {
        parent = transform.parent.gameObject;

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //parent.SendMessage("PlayerInSight");
        }
        
    }
    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            parent.SendMessage("PlayerInSight");
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            parent.SendMessage("PlayerSightLost");
        }

    }*/
}
