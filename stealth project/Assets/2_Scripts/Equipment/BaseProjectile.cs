using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{

    public Vector2 forceVector = Vector2.zero; // vector for current movement 
    public Projectile so_projectile;
    public Vector2 launchVector = Vector2.zero; // initial launch vector
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setup()
    {
        forceVector = launchVector.normalized * so_projectile.speed;
        rb = GetComponent<Rigidbody2D>();
        GetComponent<CircleCollider2D>().radius = so_projectile.hitRadius;
        if(so_projectile.sprite != null)
            GetComponent<SpriteRenderer>().sprite = so_projectile.sprite;
        Debug.Log("launch " + launchVector.normalized.ToString());
        Debug.Log("force " + forceVector.normalized.ToString());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (forceVector != Vector2.zero)
        {
            float angle = Mathf.Atan2(forceVector.y, forceVector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        

        if (forceVector.y > -so_projectile.maxFallSpeed)
            forceVector.y -= so_projectile.gravity * Time.deltaTime;
        else forceVector.y = -so_projectile.maxFallSpeed;


        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 
                                                so_projectile.hitRadius, 
                                                forceVector.normalized, 
                                                forceVector.magnitude * Time.deltaTime, 
                                                so_projectile.collisionMask);
        if (hit)
        {
            transform.position = hit.centroid;
            forceVector = Vector2.zero;

            if (so_projectile.payload != null)
            {
                GameObject pl = Instantiate(so_projectile.payload, transform.position, Quaternion.identity);
                if (so_projectile.TransferRotation)
                    pl.transform.rotation = transform.rotation;
                Destroy(this.gameObject);
            }
        }
        else
            transform.position += (Vector3)forceVector * Time.deltaTime;
    }
}
