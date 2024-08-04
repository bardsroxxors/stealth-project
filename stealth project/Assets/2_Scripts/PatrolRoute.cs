using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PatrolRoute : MonoBehaviour
{

    public Vector2[] nodes;
    //public Vector2[] nodeList;
    public int[] directions;
    public float[] waitTimes;
    public bool boomerang = true; // should the route be used as a circuit or a bommerang


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawIcon(transform.position, "circle indicator.png", false);

        // draw patrol points
        for (int i = 0; i < nodes.Length; i++) {
            Vector3 pos = transform.position;
            pos.x += nodes[i].x;
            pos.y += nodes[i].y;
            if (directions[i] == -1)
                Gizmos.DrawIcon(pos, "left arrow.png", true);
            else if (directions[i] == 1)
                Gizmos.DrawIcon(pos, "right arrow.png", true);
            pos.y += 0.3f;
            Handles.Label(pos, waitTimes[i].ToString());
        }

    }


}
