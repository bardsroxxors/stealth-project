using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_Rope : MonoBehaviour
{
    // this is the script attached to the empty that is spawned from rope shooter projectile

    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        RopeShooter shooter = GameObject.Find("RopeShooter").GetComponent<RopeShooter>();
        shooter.ProjectileHit(transform.position, CircleCast().collider.tag);
        Debug.Log("Rope hit");
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    RaycastHit2D CircleCast()
    {
        return Physics2D.CircleCast(transform.position, 0.2f, Vector2.zero, mask);
    }
}
