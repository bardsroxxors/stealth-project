using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{

    Rigidbody2D rb;
    BoxCollider2D box;
    public float throwForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickedUp() 
    { 
        box.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Dropped(Vector2 direction)
    {
        box.enabled = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
    }
}
