using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class LightScript : MonoBehaviour
{

    Light2D light;
    private float innerAngle;
    private float outerAngle;
    private float innerRadius;
    private float outerRadius;
    public float radiusDelta = 0.8f;

    private float actualRadius;
    public int sectorCount = 3;

    public GameObject player;
    private PolygonCollider2D collider;

    Utilities utils = new Utilities();


    void Start()
    {
        light = GetComponent<Light2D>();
        player = GameObject.FindWithTag("Player");
        if(light != null && light.lightType == Light2D.LightType.Point && player != null)
        {
            innerAngle = light.pointLightInnerAngle;
            outerAngle = light.pointLightOuterAngle;

            innerRadius = light.pointLightInnerRadius;
            outerRadius = light.pointLightOuterRadius;

            actualRadius = innerRadius + ((outerRadius - innerRadius) * radiusDelta);
        }

        
        SetCollider();

    }


    void Update()
    {
        float distance = (player.transform.position - transform.position).magnitude;

        if(distance < actualRadius)
        {
            //if(collider.conta)

        }

    }

    private float AngleToPlayer()
    {
        Vector3 vector = (player.transform.position - transform.position).normalized;

        return utils.GetAngleFromVectorFloat(vector);

    }


    public void SetCollider()
    {
        collider = GetComponent<PolygonCollider2D>();
        if (collider == null)
        {
            Debug.Log("Can't find collider");
            return;
        }

        float angle = utils.GetAngleFromVectorFloat(transform.up) + outerAngle/2;
        float angleIncrease = outerAngle / sectorCount;



        // vertices has +1 for the origin and +1 for point 0
        Vector3[] vertices = new Vector3[sectorCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        //int[] triangles = new int[sectorCount * 3];

        vertices[0] = Vector3.zero;

        // starting at 1 cause 0 is the origin
        int vertexIndex = 1;
        int triangleIndex = 0;



        for (int i = 0; i <= sectorCount; i++)
        {

            Ray r = new Ray(transform.position, utils.GetVectorFromAngle(angle));
            Vector3 vertex = transform.InverseTransformPoint(r.GetPoint(actualRadius));


            vertices[vertexIndex] = vertex;

            /*
            if (i > 0) // dont run this part on ray 0
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }*/
            vertexIndex++;


            // decreasing because we want to go clockwise
            angle -= angleIncrease;
        }

        
        Vector2[] points = new Vector2[vertices.Length];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = vertices[i];
        }


        collider.SetPath(0, points);

    }
}
