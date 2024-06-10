using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperdollSpriteManager : MonoBehaviour
{


    PlayerController pc;
    private int facingLastFrame = 1;

    public Vector2 swordPosition = Vector2.zero;

    // list for our paperdoll sprites
    public GameObject swordSprite;

    // Start is called before the first frame update
    void Start()
    {
        pc = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pc.playerFacingVector.x != facingLastFrame)
        {
            facingLastFrame = (int)pc.playerFacingVector.x;
        }
    }

    public void SetDollSprite(EquipSlot slot, Sprite sprite)
    {

    }
}
