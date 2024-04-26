using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavmeshGenerator : MonoBehaviour
{
    [Space]
    [SerializeField]
    public Vector2 cellSize = new Vector2(1, 1);
    public Vector3[,] pointGrid;
    public Tilemap map;

    public void Generate(Tilemap map)
    { 

        map.CompressBounds(); // shrink the map bounds to its current actual size

        int xSize = map.cellBounds.xMax - map.cellBounds.xMin;
        int ySize = map.cellBounds.yMax - map.cellBounds.yMin;


        pointGrid = new Vector3[xSize, ySize];

        Debug.Log("Iterating on map with dimensions: "+ xSize.ToString() +" "+ ySize.ToString());

        // iterate over the map
        for (int x = map.cellBounds.xMin; x < map.cellBounds.xMax; x++)
        {
            for (int y = map.cellBounds.xMin; y < map.cellBounds.xMax; y++)
            {
                //Tile tile = map
            }
        }
    }




        


    

}
