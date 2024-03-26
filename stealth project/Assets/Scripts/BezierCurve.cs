using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{

    public Vector3[] Points;

    public BezierCurve(Vector3[] points)
    {
        Points = points;
    }

    public Vector3 StartPosition
    {
        get { return Points[0]; }
    }

    public Vector3 EndPosition
    {
        get { return Points[3]; }
    }

    public Vector3 GetSegment(float Time)
    {
        Time = Mathf.Clamp01(Time);
        float t = 1 - Time;
        return (t * t * t * Points[0])
            + (3 * t * t * t * Points[1])
            + (3 * t * t * t * Points[2])
            + (t * t * t * Points[3]);
    }

    public Vector3[] GetSegments(int subdivisions)
    {
        Vector3[] segments = new Vector3[subdivisions];

        float time;
        for (int i = 0; i < subdivisions; i++)
        {
            time = (float)i / subdivisions;
            segments[i] = GetSegment(time);
        }

        return segments;
    }
}
