using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PL_Kunai : MonoBehaviour
{
    // This script acts as the middleman between the base_projectile and whatever the projectile hits.
    // Since the projectile is just based on an SO
    // this will also enable us to pick up kunai again which is cool


    public LayerMask mask_inert;
    public LayerMask mask_enemy;
    public LayerMask mask_other;
    public bool hasHit = false;

    public float hitRadius = 0.18f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasHit)
        {
            CheckForHit();
        }

    }




    private void CheckForHit()
    {
        // this lets me prioritise masks too which is epic

        
        // Check for Other collision
        RaycastHit2D hit = Physics2D.CircleCast(transform.position,
                                                hitRadius,
                                                Vector2.right,
                                                0f,
                                                mask_other);
        if (hit)
        {
            hasHit = true;
            
            return;
        }

        // Check for Enemy collision
        hit = Physics2D.CircleCast(transform.position,
                                                hitRadius,
                                                Vector2.right,
                                                0f,
                                                mask_enemy);
        if (hit)
        {
            hasHit = true;
            HitEnemy(hit.collider);
            return;
        }


        // Check for inert collision
        hit = Physics2D.CircleCast(transform.position,
                                                hitRadius,
                                                Vector2.right,
                                                0f,
                                                mask_inert);
        if (hit)
        {
            hasHit = true;
            Debug.Log("Hit inert");
            return;
        }

        
    }

    private void HitEnemy(Collider2D collider)
    {
        collider.gameObject.SendMessage("KunaiHit");
        
        DestroyImmediate(this.gameObject);
    }

}
