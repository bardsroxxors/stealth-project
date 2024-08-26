using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earpiece : MonoBehaviour
{

    EnemyStateMachine ec;

    // Start is called before the first frame update
    void Start()
    {
        ec = transform.parent.GetComponent<EnemyStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "NoiseTrigger")
        {

            NoiseScript noise = collision.gameObject.GetComponent<NoiseScript>();

            

            if (noise != null)
            {
                float reach = noise.soundSO.scaleAtDeath * 2;
                float dist = Vector3.Distance(transform.position, collision.transform.position);

                if(dist < reach)
                    ec.NoiseHeard(collision.gameObject.transform.position, noise.awarenessIncrease);
            }
                
        }

        else if (collision.gameObject.tag == "Coin")
        {
            transform.parent.SendMessage("AddBounty", 2);
            collision.gameObject.SendMessage("PickedUp");
            Destroy(collision.gameObject);
        }
        /*
        if (collision.gameObject.tag == "Door") 
        { 
            Debug.Log("door pls");
            collision.transform.gameObject.SendMessage("SetOpen", SendMessageOptions.DontRequireReceiver);
        }*/
    }
}
