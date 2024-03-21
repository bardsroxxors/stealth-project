using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldOfView : MonoBehaviour
{

    public float fov = 90f;
    public int rayCount = 2;
    public float viewDistance = 5f;
    public LayerMask layerMask;
    public Vector3 origin;
    private float startingAngle;


    // Start is called before the first frame update
    void Update()
    {

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;


        SetAimDirection(transform.right);

        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        origin = Vector3.zero;


        // vertices has +1 for the origin and +1 for ray 0
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        // starting at 1 cause 0 is the origin
        int vertexIndex = 1;
        int triangleIndex = 0;



        for(int i = 0; i<= rayCount; i++)
        {

            Ray r = new Ray(transform.position, GetVectorFromAngle(angle));
            Vector3 vertex = transform.InverseTransformPoint( r.GetPoint(viewDistance));

            RaycastHit2D raycast = Physics2D.Raycast(transform.position, GetVectorFromAngle(angle), viewDistance,  layerMask);
            if (raycast.collider != null)
            {
                // Hit object
                vertex =  transform.InverseTransformPoint( raycast.point);
            }

            vertices[vertexIndex] = vertex;


            if(i > 0) // dont run this part on ray 0
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;
            

            // decreasing because we want to go clockwise
            angle -= angleIncrease;
        }



        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;


    }



    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

}
