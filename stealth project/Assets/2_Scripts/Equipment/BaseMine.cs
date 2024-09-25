using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseMine : MonoBehaviour
{

    public Mine so_mine;
    private float t_armTime = 1f;
    public bool armed = false;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if(so_mine.disarmedSprite != null)
            GetComponent<SpriteRenderer>().sprite = so_mine.disarmedSprite;
        t_armTime = so_mine.armTime;
        //GetComponent<CircleCollider2D>().radius = so_mine.triggerRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if(t_armTime > 0f)
            t_armTime -= Time.deltaTime;
        else if (!armed)
        {
            armed = true;
            GetComponent<SpriteRenderer>().sprite = so_mine.armedSprite;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(so_mine.applyToTags.Contains(other.gameObject.tag) && armed)
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
