using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSprite : MonoBehaviour
{

    SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.GetComponent<SpriteRenderer>() != null)
        {
            renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
