using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseScript : MonoBehaviour
{

    public float lifetime = 1;
    private float t_lifetime = 0;
    public float scaleAtDeath = 1.2f;
    private float initialScale;
    public float awarenessIncrease = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        t_lifetime = lifetime;
        initialScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (t_lifetime >= 0) t_lifetime -= Time.deltaTime;
        else
        {
            Destroy(this.gameObject);
        }

        float scale = Mathf.Lerp(scaleAtDeath, initialScale, (t_lifetime / lifetime));
        transform.localScale = new Vector3(scale, scale, 1);

    }
}
