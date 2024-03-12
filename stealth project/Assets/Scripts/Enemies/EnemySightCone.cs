using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightCone : MonoBehaviour
{
    private GameObject parent;
    //public EnemyAwareness awareness;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        //awareness = parent.GetComponent<EnemyAwareness>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
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

    }
}
