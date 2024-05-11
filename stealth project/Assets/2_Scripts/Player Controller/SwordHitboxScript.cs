using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwordHitboxScript : MonoBehaviour
{

    public List<GameObject> touchingObjects = new List<GameObject>();
    public GameObject playerObject;
    // force to apply to objects
    public float objectForce = 5;

    private PlayerController playerController;

    public string[] validTags;


    private void Start()
    {
        playerController = playerObject.GetComponent<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!touchingObjects.Contains(other.gameObject))
        {
            touchingObjects.Add(other.gameObject);
            TriggerObject(other.gameObject);
        }
    }

    // trigger a hit on the given object, if it is a relevant object 
    private void TriggerObject(GameObject target)
    {
        if(target.gameObject.tag == "Enemy")
        {
            target.SendMessage("TriggerSwordHit");
            Debug.Log("diieee!");
        }

        else if (target.gameObject.tag == "SmallObject")
        {
            SmallObjectScript smlobj = target.gameObject.GetComponent<SmallObjectScript>();
            /*
            // if we hit the object we're grappling
            if ( playerController.currentTarget == target.gameObject)
            {
                playerController.GrappleBreak(target.gameObject);
                smlobj.grappled = false;
            }*/
            Vector2 forceVector = target.gameObject.transform.position - playerObject.transform.position;
            smlobj.AddForce(forceVector, objectForce);
        }
    }

    public void AttackActivated()
    {
        // check through current objects touching trigger
        for (int i = 0; i < touchingObjects.Count; i++)
        {
            TriggerObject(touchingObjects[i]);
        }
    }

    public void Deactivate()
    {
        touchingObjects = new List<GameObject> { };
    }
}
