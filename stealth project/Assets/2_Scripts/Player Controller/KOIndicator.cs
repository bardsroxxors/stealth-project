using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOIndicator : MonoBehaviour
{

    public Sprite[] sprites;
    public float animationPercent = 0;
    private SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {



        float increment = 1f / sprites.Length;
        float index = animationPercent - (animationPercent % increment);
        int i = (int) (index / increment);

        if(renderer != null)
        {
            renderer.sprite = sprites[i];
        }
    }
}
