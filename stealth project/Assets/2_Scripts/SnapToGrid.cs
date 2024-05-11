using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{

    public float gridSize = 32;

    private double x = 0;
    private double y = 0;
    private double grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = 1 / gridSize;
        x = transform.position.x;
        y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        //     Math.Round(  (value / factor)) * factor;

        x = transform.position.x - (transform.position.x % grid);
        y = transform.position.y - (transform.position.y % grid);


        transform.position = new Vector3((float)x, (float)y, transform.position.z);
    }
}
