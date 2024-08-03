using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseMine : MonoBehaviour
{

    public Mine so_mine;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if(so_mine.sprite != null)
            GetComponent<SpriteRenderer>().sprite = so_mine.sprite;
        //GetComponent<CircleCollider2D>().radius = so_mine.triggerRadius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(so_mine.applyToTags.Contains(other.gameObject.tag))
        {
            if (so_mine.directEffect)
            {
                EnemyStateMachine ec = other.gameObject.GetComponent<EnemyStateMachine>();
                if (ec != null)
                {
                    ec.ApplyCondition(so_mine.effect, so_mine.duration);
                }
                    

                
            }

                
            if(so_mine.payload != null)
            {
                GameObject payload = Instantiate(so_mine.payload, transform.position, Quaternion.identity);
                payload.SendMessage("SetDuration", so_mine.duration);
            }

            Destroy(this.gameObject);
        }
    }
}
