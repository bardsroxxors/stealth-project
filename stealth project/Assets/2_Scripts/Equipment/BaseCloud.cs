using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseCloud: MonoBehaviour
{

    public Cloud so_cloud;
    private float t_lifespan = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if(so_cloud.sprite != null)
            GetComponent<SpriteRenderer>().sprite = so_cloud.sprite;
        t_lifespan = so_cloud.lifespan;
        //GetComponent<CircleCollider2D>().radius = so_mine.triggerRadius;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        t_lifespan -= Time.deltaTime;
        if (t_lifespan < 0f) Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(so_cloud.applyToTags.Contains(other.gameObject.tag))
        {
            if (so_cloud.directEffect)
            {
                EnemyStateMachine ec = other.gameObject.GetComponent<EnemyStateMachine>();
                if (ec != null)
                {
                    ec.ApplyCondition(so_cloud.effect, so_cloud.duration);
                }
                    

                
            }

                

        }
    }
}
