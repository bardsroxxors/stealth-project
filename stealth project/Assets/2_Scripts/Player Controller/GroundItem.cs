using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


public enum ItemState
{
    world,
    bag,
    equip
}

public enum EquipSlot
{
    head,
    weapon,
    neck,
    none
}


public class GroundItem : MonoBehaviour
{
    // used to track where the item is, what state its is
    public ItemState state;
    public string world_sortingLayer = "Default";
    public string bag_sortingLayer = "UI";
    public Material world_material;
    public Material bag_material;


    public EquipSlot equipSlot = EquipSlot.none;
    public Sprite dollSprite;

    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickedUp()
    {
        gameObject.SetActive(false);
        state = ItemState.bag;

    }

    public void Dropped()
    {
        state = ItemState.world;
    }
}
