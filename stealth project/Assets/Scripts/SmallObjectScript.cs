using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SmallObjectScript : MonoBehaviour
{
    public float objectWeight = 1;
    public float objectFriction = 0.1f;
    // is the object currently grabbed?
    public bool grappled = false;
    // target position to smooth to if grpapled
    public Vector3 smoothTarget;
    // smoothing amount
    public float smoothing = 0.3f;
    public float initialAirTime = 1;
    private float TimerInitialAir = 0;

    // has this been interacted with yet?
    public bool active = false;

    private bool inAir = false;

    private Vector2 velocity = new Vector2(0, 0);

    void FixedUpdate()
    {
        if (grappled)
        {
            if(!active) active = true;
            GetSmoothToTargetVector();
        }
        if (active)
        {
            if (inAir)
            {
                TimerInitialAir -= Time.deltaTime;
                if(TimerInitialAir < 0) inAir = false;
            }
            else
            {
                // apply friction
                if (velocity.magnitude < objectFriction) velocity = Vector2.zero;
                else velocity = velocity - (velocity.normalized * objectFriction);
            }

            
            if(velocity.magnitude > 0) ApplyMovement();
            else active = false;

        }
    }




    private void ApplyMovement()
    {
        // todo make it bounce off walls
        transform.position += (Vector3)velocity * Time.deltaTime;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (velocity.magnitude > 0)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.SendMessage("TriggerForce", gameObject);
            }
        }
    }

    public void AddForce(Vector2 direction, float magnitude)
    {
        TimerInitialAir = initialAirTime;
        active = true;
        inAir = true;
        /*
        if(velocity == Vector2.zero) velocity = direction * (magnitude / objectWeight);
        else
        {
            velocity = direction.normalized * (magnitude + velocity.magnitude);
        }*/

        velocity = direction * (magnitude / objectWeight);
    }


    // used when getting dragged around to move in a smooth way
    public void GetSmoothToTargetVector()
    {
        // get direction to target
        Vector2 direction = smoothTarget - transform.position;
        // times it by the smoothing factor
        direction = direction * smoothing;
        // apply direction as movement
        velocity = direction;
    }
}
