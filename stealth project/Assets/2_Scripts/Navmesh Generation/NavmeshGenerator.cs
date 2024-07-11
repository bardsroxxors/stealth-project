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
    public GameObject[,] pointGrid;
    public Tilemap map;
    public bool drawGizmos = true;
    public LayerMask collisionLayers;
    public GameObject navPrefab;
    public GameObject NavGridParent;



    public void Generate()
    {
        // delete existing navPoint objects
        int nbChildren = NavGridParent.transform.childCount;
        if(nbChildren > 0)
        {
            for (int i = nbChildren - 1; i >= 0; i--)
            {
                DestroyImmediate(NavGridParent.transform.GetChild(i).gameObject);
            }
        }
        



        map.CompressBounds(); // shrink the map bounds to its current actual size

        int xSize = Mathf.Abs(map.cellBounds.xMax - map.cellBounds.xMin);
        int ySize = Mathf.Abs(map.cellBounds.yMax - map.cellBounds.yMin);


        pointGrid = new GameObject[xSize, ySize];

        Debug.Log("Iterating on map with dimensions: "+ xSize.ToString() +" "+ ySize.ToString());


        int px = 0;
        int py = 0;

        int platformIndex = 0;
        bool platformStarted = false;


        // #####################   Create the NavPoints, assign them a platform type and platform index   #####################
        for (int y = map.cellBounds.yMin; y < map.cellBounds.yMax; y++)
        {
            px = 0;
            platformStarted = false;

            // for each column in row
            for (int x = map.cellBounds.xMin; x < map.cellBounds.xMax; x++)
            {

                // if there is free space and a tile below
                if (map.GetSprite(new Vector3Int(x, y, 0)) == null &&
                    map.GetSprite(new Vector3Int(x, y - 1, 0)) != null &&
                    map.GetSprite(new Vector3Int(x, y + 1, 0)) == null)
                {

                    Vector3 pos = map.layoutGrid.CellToWorld(new Vector3Int(x, y, 0));
                    pos = new Vector3(pos.x, pos.y + 0.5f, pos.z);

                    Vector3 platformLastPos = map.layoutGrid.CellToWorld(new Vector3Int(x, y, 0));
                    platformLastPos = new Vector3(platformLastPos.x+1, platformLastPos.y + 0.5f, platformLastPos.z);

                    GameObject nav = Instantiate(navPrefab, pos, Quaternion.identity, NavGridParent.transform);

                    // #########  Solo Tile  #########
                    // if there is no sprite to lower left or right
                    // OR there is a sprite to immediate left and right
                    if ((map.GetSprite(new Vector3Int(x - 1, y - 1, 0)) == null &&
                        map.GetSprite(new Vector3Int(x + 1, y - 1, 0)) == null) 
                        ||
                        (map.GetSprite(new Vector3Int(x - 1, y, 0)) != null &&
                        map.GetSprite(new Vector3Int(x + 1, y, 0)) != null))
                    {
                        nav.GetComponent<NavPoint>().type = NavpointType.leftEdge;
                        nav.GetComponent<NavPoint>().platformIndex = 0;

                        // Create extra point for right edge
                        GameObject right = Instantiate(navPrefab, platformLastPos, Quaternion.identity, NavGridParent.transform);
                        right.GetComponent<NavPoint>().type = NavpointType.rightEdge;
                        right.GetComponent<NavPoint>().platformIndex = platformIndex + 1;

                        platformIndex = 0;
                    }

                    // #########  Left Edge Tile  #########
                    // if there is no sprite to lower left
                    // OR there is a sprite to immediate left
                    else if (map.GetSprite(new Vector3Int(x - 1, y - 1, 0)) == null ||
                            map.GetSprite(new Vector3Int(x - 1, y, 0)) != null)
                    {
                        nav.GetComponent<NavPoint>().type = NavpointType.leftEdge;
                        nav.GetComponent<NavPoint>().platformIndex = platformIndex;
                        platformIndex++;
                    }
                    // #########  Right Edge Tile  #########
                    // if there is no sprite to lower right
                    // OR there is a sprite to immediate right
                    else if (map.GetSprite(new Vector3Int(x+1, y-1, 0)) == null ||
                            map.GetSprite(new Vector3Int(x + 1, y, 0)) != null)
                    {
                        nav.GetComponent<NavPoint>().type = NavpointType.platform;
                        nav.GetComponent<NavPoint>().platformIndex = platformIndex;
                        

                        // Create extra point for right edge
                        GameObject right = Instantiate(navPrefab, platformLastPos, Quaternion.identity, NavGridParent.transform);
                        right.GetComponent<NavPoint>().type = NavpointType.rightEdge;
                        right.GetComponent<NavPoint>().platformIndex = platformIndex +1;

                        platformIndex = 0;
                    }
                    // #########  Normal Tile  #########
                    else
                    {
                        nav.GetComponent<NavPoint>().type = NavpointType.platform;
                        nav.GetComponent<NavPoint>().platformIndex = platformIndex;
                        platformIndex++;
                    }
                    
                    nav.GetComponent<NavPoint>().gridPosition = new Vector3Int(x, y, 0);
                    pointGrid[px, py] = nav;

                }

                else
                {
                    platformIndex = 0;
                }

                px++;
            }
            py++;
        }
        // #####   ---------                                 - - - -                                        ---------     #####

        /*
        // create the navLinks
        for (int y = 0; y < pointGrid.GetLength(0); y++)
        {
            for (int x = 0; x < pointGrid.GetLength(1); x++)
            {
                if (pointGrid[x, y] != null)
                {

                }

            }
        }
        */

        Debug.Break();
    }

    
    private void OnDrawGizmos()
    {


        if (drawGizmos == false)
        {
            return;
        }


        Handles.color = UnityEngine.Color.yellow;


        if(pointGrid != null)
        {
            // instead, iterate over the array of navpoints
            for (int x = 0; x < pointGrid.GetLength(0); x++)
            {
                for (int y = 0; y < pointGrid.GetLength(1); y++)
                {
                    if(pointGrid[x, y] != null) {
                        Vector3 pos = pointGrid[x, y].transform.position;

                        if (pointGrid[x, y].GetComponent<NavPoint>().type == NavpointType.platform)
                            Handles.color = UnityEngine.Color.yellow;
                        if (pointGrid[x, y].GetComponent<NavPoint>().type == NavpointType.leftEdge)
                            Handles.color = UnityEngine.Color.green;
                        if (pointGrid[x, y].GetComponent<NavPoint>().type == NavpointType.rightEdge)
                            Handles.color = UnityEngine.Color.blue;
                        if (pointGrid[x, y].GetComponent<NavPoint>().type == NavpointType.solo)
                            Handles.color = UnityEngine.Color.cyan;

                        Handles.DrawWireCube(pos, new Vector3(0.25f, 0.25f, 0.25f));
                    }
                    
                }
            }
        }

        
    }






}
