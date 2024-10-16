using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_Rope : MonoBehaviour
{
    // this is the script attached to the empty that is spawned from rope shooter projectile


    // Start is called before the first frame update
    void Start()
    {
        GameObject shooter = GameObject.Find("RopeShooter");
        shooter.SendMessage("ProjectileHit", transform.position);
        Debug.Log("Rope hit");
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
