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

    public float triggerWidth = 0.5f;

    public RopeMode ropeMode = RopeMode.none;

    public Vector2 currentDirection = Vector2.zero;

    private LineRenderer lineRenderer;
    private Utilities utils = new Utilities();
    PolygonCollider2D collider;

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
        collider = GetComponent<PolygonCollider2D>();

        lineRenderer = GetComponent<LineRenderer>();

        pos1 = p1;
        pos2 = p2;

        // set them past the positions so they go into the wall
        // get normalised vector * small amount
        // add it to one and subtract from the other

        currentDirection = (pos1 - pos2).normalized;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pos1);
        lineRenderer.SetPosition(1, pos2);


        // set positions for collider
        SetPolygonCollider();
    }


    private void SetPolygonCollider()
    {
        Vector2[] points = new Vector2[4];

        // Get normalised vector for angle of rope

        points[0] = (Vector2)pos1 + Vector2.Perpendicular(currentDirection) * triggerWidth;
        points[1] = (Vector2)pos1 - Vector2.Perpendicular(currentDirection) * triggerWidth;
        points[2] = (Vector2)pos2 - Vector2.Perpendicular(currentDirection) * triggerWidth;
        points[3] = (Vector2)pos2 + Vector2.Perpendicular(currentDirection) * triggerWidth;

        points[0] = transform.InverseTransformPoint(points[0]);
        points[1] = transform.InverseTransformPoint(points[1]);
        points[2] = transform.InverseTransformPoint(points[2]);
        points[3] = transform.InverseTransformPoint(points[3]);

        Debug.Log((Vector2)pos1 + Vector2.Perpendicular(currentDirection) * triggerWidth);

        collider.SetPath(0, points);
    }

    // this is used to get the nearest point on the line to the parameter position
    public Vector3 GetNearestPoint(Vector3 target)
    {
        return NearestPointOnLine(pos1, (pos1 - pos2).normalized, target);
    }

    //linePnt - point the line passes through
    //lineDir - unit vector in direction of line, either direction works
    //pnt - the point to find nearest on line for
    private static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }

}
