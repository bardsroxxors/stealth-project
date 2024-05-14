using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotParticle : MonoBehaviour
{
    float lifetime = 0.1f;
    float t_lifetime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        t_lifetime = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (t_lifetime >= 0) t_lifetime -= Time.deltaTime;
        else Destroy(this.gameObject);
    }
}
