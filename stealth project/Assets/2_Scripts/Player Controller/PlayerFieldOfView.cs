using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static System.DateTime;


public class PlayerFieldOfView : MonoBehaviour
{

    public float fov = 90f;
    public int rayCount = 2;
    public float viewDistance = 5f;
    public LayerMask layerMask;
    public Vector3 origin;
    private float startingAngle;

    private Utilities utils = new Utilities();
    private GameObject EnemyObject;
    public SpriteRenderer spriteR;


    private void Start()
    {



        Debug.Log("what?");

        spriteR = gameObject.GetComponent<SpriteRenderer>();
        
            

        EnemyObject = transform.parent.parent.gameObject;
    }


    // Start is called before the first frame update
    void Update()
    {
        /*
        System.DateTime before = System.DateTime.Now;*/
        transform.localPosition = Vector3.zero;
        


        SetAimDirection(transform.right);

        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        origin = Vector3.zero;


        // vertices has +1 for the origin and +1 for ray 0
        Vector2[] vertices = new Vector2[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        ushort[] triangles = new ushort[rayCount * 3];

        vertices[0] = origin;

        // starting at 1 cause 0 is the origin
        int vertexIndex = 1;
        int triangleIndex = 0;



        for(int i = 0; i<= rayCount; i++)
        {

            Ray r = new Ray(transform.position, utils.GetVectorFromAngle(angle));
            Vector3 vertex = transform.InverseTransformPoint( r.GetPoint(viewDistance));

            RaycastHit2D raycast = Physics2D.Raycast(transform.position, utils.GetVectorFromAngle(angle), viewDistance,  layerMask);
            if (raycast.collider != null)
            {
                // Hit object
                vertex =  transform.InverseTransformPoint( raycast.point);
            }

            vertices[vertexIndex] = vertex;


            if(i > 0) // dont run this part on ray 0
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = (ushort)(vertexIndex - 1);
                triangles[triangleIndex + 2] = (ushort)vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;
            

            // decreasing because we want to go clockwise
            angle -= angleIncrease;
        }


        /*
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;*/

        Debug.Log(spriteR.sprite.ToString());
        
        spriteR.sprite.OverrideGeometry(vertices, triangles);
        


        /*
        System.DateTime after = System.DateTime.Now;
        System.TimeSpan duration = after.Subtract(before);
        Debug.Log("Duration in milliseconds: " + duration.Milliseconds);*/

    }





    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = utils.GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }





}
