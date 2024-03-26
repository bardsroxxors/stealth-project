using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotIndicatorCollision : MonoBehaviour
{
    public bool isColliding = false;



    private void OnTriggerStay2D(Collider2D collision)
    {
        isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }

}
