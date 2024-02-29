using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleLineRenderer : MonoBehaviour
{

    private LineRenderer lr;
    private Transform[] points = {};
    public bool FlagDraw = false;


    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetUpLine(Transform[] points)
    {
        lr.positionCount = points.Length;
        this.points = points;
    }

    private void Update()
    {
        if (FlagDraw)
        {
            if (!lr.enabled) lr.enabled = true;

            for (int i = 0; i < points.Length; i++)
            {
                lr.SetPosition(i, points[i].position);
            }
        }
        else
        {
            if (lr.enabled) lr.enabled = false;
        }
        
    }

}
