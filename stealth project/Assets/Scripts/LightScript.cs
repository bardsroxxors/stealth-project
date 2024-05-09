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

    public GameObject player;

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

            actualRadius = outerRadius + ((outerRadius - innerRadius) * radiusDelta);
        }
    }


    void Update()
    {
        float distance = (player.transform.position - transform.position).magnitude;

        if(distance < actualRadius)
        {
            float center = utils.GetAngleFromVectorFloat(transform.up);
            Vector2 coneAngles = new Vector2(center + (outerAngle / 2), center - (outerAngle / 2));
            float playerAngle = AngleToPlayer();

            //if (playerAngle <= )

            Debug.Log(AngleToPlayer());

        }

    }

    private float AngleToPlayer()
    {
        Vector3 vector = (player.transform.position - transform.position).normalized;

        return utils.GetAngleFromVectorFloat(vector);

    }
}
