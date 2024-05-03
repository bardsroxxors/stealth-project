using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavmeshGenerator : MonoBehaviour
{
    [Space]
    [SerializeField]
    public Vector2 cellSize = new Vector2(1, 1);
    public NavPoint[,] pointGrid;
    public Tilemap map;
    public bool drawGizmos = true;
    public LayerMask collisionLayers;
    public GameObject thing;


    private void Update()
    {
        Generate();
    }

    public void Generate()
    { 

        map.CompressBounds(); // shrink the map bounds to its current actual size

        int xSize = map.cellBounds.xMax - map.cellBounds.xMin+1;
        int ySize = map.cellBounds.yMax - map.cellBounds.yMin+1;


        pointGrid = new NavPoint[xSize, ySize];

        Debug.Log("Iterating on map with dimensions: "+ xSize.ToString() +" "+ ySize.ToString());


        int px = 0;
        int py = 0;

        // iterate over the map
        for (int x = map.cellBounds.xMin; x < map.cellBounds.xMax; x++)
        {
            for (int y = map.cellBounds.yMin; y < map.cellBounds.yMax; y++)
            {

                NavPoint p = new NavPoint();

                Vector3 pos = map.layoutGrid.CellToWorld(new Vector3Int(x, y, 0));
                pos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z);

                Collider2D hit = Physics2D.OverlapBox(
                    pos,
                    new Vector2(0.25f, 0.25f),
                    0,
                    collisionLayers,
                    -10, 10);



                if (hit != null)
                {
                    p.type = NavpointType.platform;
                    Instantiate(thing, pos, Quaternion.identity);
                }
                else p.type = NavpointType.none;


                Debug.Log(x.ToString() +" "+ y.ToString());
                pointGrid[px, py] = p;

                py++;

            }

            px++;
        }
    }

    /*
    private void OnDrawGizmos()
    {


        if (drawGizmos == false)
        {
            return;
        }


        Handles.color = UnityEngine.Color.yellow;



        map.CompressBounds(); // shrink the map bounds to its current actual size

        int xSize = map.cellBounds.xMax - map.cellBounds.xMin;
        int ySize = map.cellBounds.yMax - map.cellBounds.yMin;


        int px = 0;
        int py = 0;

        // iterate over the map
        for (int x = map.cellBounds.xMin; x < map.cellBounds.xMax; x++)
        {
            for (int y = map.cellBounds.yMin; y < map.cellBounds.yMax; y++)
            {

                Vector3 pos = map.layoutGrid.CellToWorld(new Vector3Int(x, y, 0));
                pos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z);

                if (pointGrid != null)
                {
                    NavPoint p = pointGrid[px, py];

                    if(p.type == NavpointType.platform) Handles.color = UnityEngine.Color.red;
                    else Handles.color = UnityEngine.Color.yellow;
                }



                Handles.DrawWireCube(
                            pos,
                            new Vector3(
                                0.25f,
                                0.25f,
                                0.25f));
                py++;
            }
            px++;
        }
    }*/






}
