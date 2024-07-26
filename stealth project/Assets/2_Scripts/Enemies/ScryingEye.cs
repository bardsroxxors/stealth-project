using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using static UnityEngine.GraphicsBuffer;
using System;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;
using System.Linq;
using static UnityEditor.PlayerSettings;
using UnityEditor;

public class ScryingEye : MonoBehaviour
{
    private Utilities utils = new Utilities();

    // gotta make this guy pan around I guess
    // set angles
    public float angle1;
    public float angle2;
    private float t = 0;

    public float lerpSpeed = 0.1f;
    public float waitTime = 1f;
    private float t_waitTime = 0;
    private int direction = 1;

    private Quaternion quat1 = Quaternion.identity;
    private Quaternion quat2 = Quaternion.identity;


    // Start is called before the first frame update
    void Start()
    {
        quat1 = Quaternion.AngleAxis(angle1, Vector3.forward);
        quat2 = Quaternion.AngleAxis(angle2, Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Slerp(quat1, quat2, t);

        // if we are waiting
        if(t_waitTime > 0 ) t_waitTime -= Time.deltaTime;
        // else if we are in between
        else if(t >= 0 && t <= 1)
        {
            t += lerpSpeed * direction;
        }
        // else we get to an edge
        else
        {
            direction *= -1;
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            t_waitTime = waitTime;
        }
    }



    private void OnDrawGizmos()
    {

        Debug.DrawRay(transform.position, utils.GetVectorFromAngle(angle1) * 2);
        Debug.DrawRay(transform.position, utils.GetVectorFromAngle(angle2) * 2);
    }
}
