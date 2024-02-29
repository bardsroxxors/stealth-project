using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrappleHitboxScript : MonoBehaviour
{
    public List<GameObject> touchingObjects = new List<GameObject>();
    public GameObject playerObject;
    // empty gameobject used when grappling a wall
    public GameObject wallGrabPoint;
    public float range = 9;
    

    public string[] validTags;

    private void Start()
    {
        touchingObjects.Add(wallGrabPoint);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!touchingObjects.Contains(other.gameObject) && validTags.Contains( other.gameObject.tag))
        {
            touchingObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (touchingObjects.Contains(other.gameObject))
        {
            touchingObjects.Remove(other.gameObject);
        }
    }




    // return the most likely target from the objects touching the trigger
    public GameObject GetTarget()
    {
        bool wallHit = CastWallRay();

        float closestDistance = 150;
        GameObject target = null;

        // get the closest object from the list
        foreach (GameObject obj in touchingObjects)
        {
            float distance = Vector3.Distance(playerObject.transform.position, obj.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = obj;
            }
        }


        

        if (target != wallGrabPoint || wallHit)
        {
            return target;
        }
        else
        {
            Debug.Log("No objects in the list.");
            return null;
        }
    }


    public bool CastWallRay()
    {
        LayerMask mask = LayerMask.GetMask("Walls");

        RaycastHit2D ray = Physics2D.Raycast(playerObject.transform.position, transform.right, range, mask);
        Debug.DrawRay(playerObject.transform.position, transform.right * range, Color.blue, 1);
        

        if(ray.collider != null)
        {
            if(Vector2.Distance(transform.position, ray.point) < range)
            {
                Debug.Log(ray.collider.gameObject.tag);
                if (ray.collider.gameObject.tag == "Wall")
                {
                    wallGrabPoint.transform.position = ray.point;
                    return true;
                }
            }
            
        }
        return false;
    }


}
