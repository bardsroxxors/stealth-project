using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_ForceMine : MonoBehaviour
{
    // The force mine is applies force to stuff when it's triggered.
    // It has an effect radius and affects everything that's in the radius when it goes off

    public float radius = 3;
    public Vector2 force = new Vector2(5, 5);
    public LayerMask mask_effected;
    public Sound sound;
    public GameObject noisePrefab;

    void Start()
    {
        
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.right, 0, mask_effected);
        
        foreach(RaycastHit2D hit in hits)
        {
            GameObject noise = Instantiate(noisePrefab, transform.position, Quaternion.identity);
            noise.SendMessage("SetProfile", sound);

            Vector2 f = new Vector2(Mathf.Sign(hit.point.x - transform.position.x) * force.x, force.y);

            EnemyStateMachine ec = hit.collider.gameObject.GetComponent<EnemyStateMachine>();
            if (ec != null) {
                //ec.ApplyForce(f);
                hit.collider.gameObject.SendMessage("ApplyForce", f);
            }

        }

        Destroy(this.gameObject);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
