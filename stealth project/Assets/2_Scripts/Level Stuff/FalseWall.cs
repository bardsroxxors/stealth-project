using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Disappear()
    {
        if (transform.parent != null && transform.parent.gameObject.layer == 3)
            Destroy(transform.parent.gameObject);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {/*
        if(collision.gameObject.tag == "Player")
        {
            if (transform.parent != null && transform.parent.gameObject.layer == 3)
                Destroy(transform.parent.gameObject);
            Destroy(this.gameObject);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "PlayerProjectile")
        {
            Disappear();
        }
    }


}
