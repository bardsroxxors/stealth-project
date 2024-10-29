using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum RopeMode
{
    none,
    wallToWall,
    wallToObject
}


public class RopeScript : MonoBehaviour
{
    // this script is for the actual rope that spawns in the world


    public Vector3 pos1 = Vector3.zero;
    public Vector3 pos2 = Vector3.zero;

    public string tag1 = "";
    public string tag2 = "";

    public RopeMode ropeMode = RopeMode.none;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Vector3 p1, Vector3 p2, string t1, string t2)
    {
        lineRenderer = GetComponent<LineRenderer>();

        pos1 = p1;
        pos2 = p2;

        // set them past the positions so they go into the wall
        // get normalised vector * small amount
        // add it to one and subtract from the other

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pos1);
        lineRenderer.SetPosition(1, pos2);


        // set positions for collider

    }

    
}
