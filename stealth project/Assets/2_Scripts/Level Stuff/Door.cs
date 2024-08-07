using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : MonoBehaviour
{

    public bool open = false;
    BoxCollider2D collider;
    SpriteRenderer sprite;
    ShadowCaster2D shadow;
    public Sprite openSprite;
    public Sprite closedSprite;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        shadow = GetComponent<ShadowCaster2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleOpen()
    {
        open = !open;
        if (open)
        {
            collider.enabled = false;
            shadow.enabled = false;
            sprite.sprite = openSprite;
        }
        else
        {
            collider.enabled = true;
            shadow.enabled = true;
            sprite.sprite = closedSprite;
        }
    }
}
