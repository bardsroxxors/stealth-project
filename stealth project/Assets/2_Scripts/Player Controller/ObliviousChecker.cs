using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObliviousChecker : MonoBehaviour
{

    // this is checks to see if the player moves past ana enemy in a way that whey are oblivious
    // this means if the enemy is not alert and moves past or under the player

    // i want it to trigger if you move over them
    // if you're hidden in a barrel and they move past
    // if you're on a wall or grabpoint and they move under you
    // only with a direct path between enemy and player

    public LayerMask mask;
    Vector3 hitPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, mask);
        if (hit)
        {
            hitPos = hit.point;
            Debug.DrawRay(transform.position, Vector2.down * hit.distance);
            if (hit.collider.gameObject.tag == "Enemy")
            {
                hit.collider.gameObject.SendMessage("ObliviousIdiot");
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (hitPos != null)
        {
            Gizmos.DrawWireCube(hitPos, new Vector3(0.2f,0.2f,0.2f));
        }
    }
}
