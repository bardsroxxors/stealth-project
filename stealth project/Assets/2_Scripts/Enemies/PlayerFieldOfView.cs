using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static System.DateTime;


public class PlayerFieldOfView : MonoBehaviour
{
    public bool isPlayer = false;


    public float xRayOffset = 1f;
    public float yRayOffset = 1f;
    public Vector2 originOffset = new Vector2(1,1);
    public float fov = 90f;
    public int rayCount = 2;
    public float viewDistance = 5f;
    public float maxDistance = 40f;
    public float pierceDistance = 0.5f;
    public LayerMask obstructMask;
    public LayerMask boundingMask;
    public Vector3 origin;
    private float startingAngle;
    private Mesh mesh;
    //private PolygonCollider2D collider;
    private GameObject partialShadowObj;

    private Utilities utils = new Utilities();
    private GameObject playerObject;
    private PlayerController playerController;

    SpriteRenderer sprite;


    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        partialShadowObj = transform.GetChild(0).gameObject;
        partialShadowObj.GetComponent<MeshFilter>().mesh = mesh;
        //collider = GetComponent<PolygonCollider2D>();

        //SetPolygonCollider();

        if (isPlayer)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
            

        playerObject = transform.parent.gameObject;
        playerController = playerObject.GetComponent<PlayerController>();
    }


    // Start is called before the first frame update
    void Update()
    {
        DrawRays();


        /*
        System.DateTime before = System.DateTime.Now;*/
        transform.localPosition = Vector3.zero;
        


        SetAimDirection(transform.right);

        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        origin = Vector3.zero;


        // vertices has +1 for the origin and +1 for ray 0
        // vertices has 2 per ray, and plus 2 for ray 0
        Vector3[] vertices = new Vector3[(rayCount * 2) + 2 +1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 6];

        vertices[0] = origin;

        // starting at 1 cause 0 is the origin
        int vertexIndex = 1;
        int triangleIndex = 0;

        //Debug.Log("Vertices:"+vertices.Length);

        for (int i = 0; i<= rayCount; i++)
        {


            Ray r = new Ray(transform.position, utils.GetVectorFromAngle(angle));
            Vector3 innerVert = transform.InverseTransformPoint( r.GetPoint(viewDistance));
            Vector3 outerVert = transform.InverseTransformPoint(r.GetPoint(maxDistance));

            


            RaycastHit2D raycast = Physics2D.Raycast(transform.position, utils.GetVectorFromAngle(angle), viewDistance,  obstructMask);
            if (raycast.collider != null)
            {
                // Hit object
                innerVert =  transform.InverseTransformPoint( raycast.point);

            }

            //Debug.Log("Vert Index 1:" + vertexIndex);
            vertices[vertexIndex] = innerVert;
            vertexIndex++;
            //Debug.Log("Vert Index 2:" + vertexIndex);
            vertices[vertexIndex] = outerVert;

            if (i > 0) // dont run this part on ray 0
            {
                triangles[triangleIndex + 0] = vertexIndex - 3;
                triangles[triangleIndex + 1] = vertexIndex - 2;
                triangles[triangleIndex + 2] = vertexIndex - 1;

                triangles[triangleIndex + 3] = vertexIndex - 1;
                triangles[triangleIndex + 4] = vertexIndex - 2;
                triangles[triangleIndex + 5] = vertexIndex;

                triangleIndex += 6;
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

    private void DrawRays()
    {
        // to move the centre point around we need o make sure all the rays are coming from that point
        // how do we check that we need to move it?
        // I want it to move when the player is close to an edge
        // that will be dependent on the direction of the surface they're on
        // so if standing on the ground, move it left or right
        // if on a wall, move it up and down
        // use raycasts that start beyond the main collider to check if there is open space

        // so we want to get the collision directions from the player object
        // then depending on that we will draw some rays to check where the edge is at
        // we draw the rays from the center, offset outwards, pointing in the direction of 
        // the collision surface
        // but what about if there are two collisions?
        // worry about that later
        Vector2 colDir = playerController.collisionDirections;

        // if standing on the ground
        if(colDir.y == -1)
        {
            Debug.DrawRay(transform.position + new Vector3(xRayOffset, 0, 0), Vector3.down);
            Debug.DrawRay(transform.position + new Vector3(-xRayOffset, 0, 0), Vector3.down);
        }

        if (colDir.x == 1)
        {
            Debug.DrawRay(transform.position + new Vector3(0, yRayOffset, 0), Vector3.left);
            Debug.DrawRay(transform.position + new Vector3(0, -yRayOffset, 0), Vector3.left);
        }
        else if (colDir.x == -1)
        {
            Debug.DrawRay(transform.position + new Vector3(0, yRayOffset, 0), Vector3.right);
            Debug.DrawRay(transform.position + new Vector3(0, -yRayOffset, 0), Vector3.right);
        }

        //Debug.DrawRay(transform.position + new Vector3(xRayOffset,0,0), Vector3.down);
        //Debug.DrawRay(transform.position + new Vector3(xRayOffset, 0, 0), Vector3.up);
        //Debug.DrawRay(transform.position + new Vector3(0, yRayOffset, 0), Vector3.left);
        //Debug.DrawRay(transform.position + new Vector3(0, yRayOffset, 0), Vector3.right);
    }

    // set up the polygon collider shape
    // origin, transform.right + 
    private void SetPolygonCollider()
    {
        Vector2[] points = new Vector2[3];

        points[0] = Vector2.zero;
        points[1] = utils.GetVectorFromAngle((fov / 2)) * viewDistance;
        points[2] = utils.GetVectorFromAngle(-(fov / 2)) * viewDistance;

        //collider.SetPath(0, points);
    }



    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = utils.GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }
    /*

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc && pc.CurrentPlayerState != e_PlayerControllerStates.Hiding)
            {
                playerObject.SendMessage("PlayerInSight", SendMessageOptions.DontRequireReceiver);
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
                playerObject.SendMessage("PlayerSightLost");
            }
            
        }

    }
    */

}
