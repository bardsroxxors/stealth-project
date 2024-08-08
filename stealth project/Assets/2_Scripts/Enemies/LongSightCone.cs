using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongSightCone : MonoBehaviour
{
    public float fov = 90f;
    public float viewDistance = 5f;
    private Utilities utils = new Utilities();
    private PolygonCollider2D collider;
    private GameObject EnemyObject;



    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<PolygonCollider2D>();

        SetPolygonCollider();

        EnemyObject = transform.parent.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // set up the polygon collider shape
    // origin, transform.right + 
    private void SetPolygonCollider()
    {
        Vector2[] points = new Vector2[4];

        points[0] = new Vector2(0, -1);
        points[1] = new Vector2(0, 1);
        points[2] = utils.GetVectorFromAngle((fov / 2)) * viewDistance;
        points[3] = utils.GetVectorFromAngle(-(fov / 2)) * viewDistance;

        collider.SetPath(0, points);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if(player != null)
            {
                if (player.lit)
                {
                    EnemyObject.SendMessage("PlayerInSight");
                }
                else EnemyObject.SendMessage("PlayerSightLost", 0, SendMessageOptions.DontRequireReceiver);
                    
            }

            
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            EnemyObject.SendMessage("PlayerSightLost");
        }

    }
}
