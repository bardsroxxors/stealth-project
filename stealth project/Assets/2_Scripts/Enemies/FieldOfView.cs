using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static System.DateTime;


public class FieldOfView : MonoBehaviour
{
    public bool isPlayer = false;

    public float fov = 90f;
    public int rayCount = 2;
    public float viewDistance = 5f;
    public LayerMask layerMask;
    public Vector3 origin;
    private float startingAngle;
    private Mesh mesh;
    private PolygonCollider2D collider;

    private Utilities utils = new Utilities();
    private GameObject EnemyObject;
    SpriteRenderer sprite;


    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        collider = GetComponent<PolygonCollider2D>();

        SetPolygonCollider();

        if (isPlayer)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
            

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
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

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

        /*
        if (isPlayer)
        {
            Vector2[] verts = vertices;
            sprite.sprite.OverrideGeometry((Vector2[])vertices, triangles);
        }*/


        /*
        System.DateTime after = System.DateTime.Now;
        System.TimeSpan duration = after.Subtract(before);
        Debug.Log("Duration in milliseconds: " + duration.Milliseconds);*/

    }



    // set up the polygon collider shape
    // origin, transform.right + 
    private void SetPolygonCollider()
    {
        Vector2[] points = new Vector2[3];

        points[0] = Vector2.zero;
        points[1] = utils.GetVectorFromAngle((fov / 2)) * viewDistance;
        points[2] = utils.GetVectorFromAngle(-(fov / 2)) * viewDistance;

        collider.SetPath(0, points);
    }



    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = utils.GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc && pc.CurrentPlayerState != e_PlayerControllerStates.Hiding)
            {
                EnemyObject.SendMessage("PlayerInSight", SendMessageOptions.DontRequireReceiver);
            }
            
        }
        else if (collision.gameObject.tag == "Corpse")
        {

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc && pc.CurrentPlayerState != e_PlayerControllerStates.Hiding)
            {
                EnemyObject.SendMessage("PlayerSightLost");
            }
            
        }

    }


}
