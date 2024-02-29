using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteManager : MonoBehaviour
{



    public Vector2 playerFacing = new Vector2(1, 0);

    public enum PlayerAnimationState
    {
        Walk,
        Dash,
        lightSlash,
        heavySlash
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
