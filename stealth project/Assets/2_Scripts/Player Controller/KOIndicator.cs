using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOIndicator : MonoBehaviour
{

    public Sprite[] sprites;
    public float animationPercent = 0;
    private SpriteRenderer renderer;
    public Vector3 targetPosition;
    public float minZipDistance = 0.1f;
    public float zipSpeed = 15;



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
        /*
        Vector2 distance = targetPosition - transform.position;
        if (distance.magnitude > minZipDistance)
        {
            transform.Translate(distance.normalized * zipSpeed, Space.World);
        }*/

        transform.position = targetPosition;    
    }
}
