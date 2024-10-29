using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeShooter : MonoBehaviour
{

    public GameObject pos1;
    public GameObject pos2;
    public GameObject ropePrefab;

    public string tag1;
    public string tag2;

    //public float circleCastWidth = 0.5f;
    public float maxDistance = 10f;
    public float minDistance = 2f;
    public float castBufferDistance = 1f;

    public bool halfShot = false;

    public LayerMask castMask;

    private Utilities utils = new Utilities();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // function to recieve message from projectile
    // it sets the positions 

    public void ProjectileHit(Vector3 position, string tag)
    {
        if (!halfShot)
        {
            pos1.transform.position = position;
            tag1 = tag;
            halfShot = true;
        }
        else
        {
            pos2.transform.position = position;
            tag2 = tag;
            CreateRope();
        }
    }

    // create the rope thing using our 2 positions
    public void CreateRope()
    {
        halfShot = false;
        if (CheckValid())
        {
            GameObject rope = Instantiate(ropePrefab, pos1.transform.position, Quaternion.identity);
            rope.GetComponent<RopeScript>().Setup(pos1.transform.position, pos2.transform.position, tag1, tag2);
        }

        Debug.Log("Create rope attempt" + tag1 + tag2);
    }

    private bool CheckValid()
    {
        // check that the 2 points have LOS to each other and are in range

        // check that distance is within min
        float dist = Vector3.Distance(pos1.transform.position, pos2.transform.position);
        if (dist > maxDistance)
        {
            Debug.Log("Too far");
            return false;
        }

        if (dist < minDistance)
        {
            Debug.Log("Too short");
            return false;
        }

        // get direction from pos1 to pos2
        // do a circle cast in that direction
        // start and end a little away from the wall so you don't hit the wall it's stuck to

        // dir from 1 to 2
        Vector3 direction = (pos2.transform.position - pos1.transform.position).normalized;

        RaycastHit2D cast = Physics2D.Raycast(      pos1.transform.position,
                                                    direction, 
                                                    maxDistance,
                                                    castMask);

        //Debug.DrawRay(pos1.transform.position + (direction * castBufferDistance), direction);
        //Debug.Break();

        if (cast)
        {
            Debug.Log("Raycast failed");
            //return false;
        }

        return true;
    }
}
