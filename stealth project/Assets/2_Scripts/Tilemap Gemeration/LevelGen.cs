using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGen : MonoBehaviour
{

    public Tilemap frontMap;
    public Tilemap backMap;
    public TileBase wall;
    public TileBase backWall;
    public Texture2D frontTemplate;
    //public Texture2D backTemplate;
    public Color wallColor;
    public Color lightColor;
    public Color backLayerColor;
    private GameObject lightsContainer;
    public GameObject hangingLight;
    public GameObject standingLight;
    public Color backdropColor;
    public GameObject backdropPlane;


    public void Generate()
    {

        frontMap.ClearAllTiles();
        backMap.ClearAllTiles();
        DestroyImmediate(lightsContainer);

        lightsContainer = new GameObject("Lights");
        lightsContainer.transform.parent = this.transform;

        for (int y = 0; y < (int)frontTemplate.height; y++)
        {


            // for each column in row
            for (int x = 0; x < (int)frontTemplate.width; x++)
            {
                // walls
                if (frontTemplate.GetPixel(x, y) == wallColor)
                {
                    frontMap.SetTile(new Vector3Int(x, y, 0), wall);
                    backMap.SetTile(new Vector3Int(x, y, 0), backWall);
                }
                // lights
                else if(frontTemplate.GetPixel(x, y) == lightColor)
                {
                    GameObject light = null;
                    if (frontTemplate.GetPixel(x, y + 1) == wallColor)
                        light = hangingLight;
                    else if (frontTemplate.GetPixel(x, y - 1) == wallColor)
                        light = standingLight;

                    if(light != null)
                        Instantiate(light,
                                    frontMap.GetCellCenterWorld(new Vector3Int(x, y, 0)),
                                    Quaternion.identity,
                                    lightsContainer.transform);

                    if(DetermineBackTile(x, y))
                        backMap.SetTile(new Vector3Int(x, y, 0), backWall);

                }


                // back layer
                if (frontTemplate.GetPixel(x, y) == backLayerColor)
                {
                    backMap.SetTile(new Vector3Int(x, y, 0), backWall);
                }

            }

        }

        backdropPlane.transform.position = frontMap.cellBounds.center;
        backdropPlane.transform.localScale = new Vector3(frontMap.cellBounds.xMax - frontMap.cellBounds.xMin, 
                                                        frontMap.cellBounds.yMax - frontMap.cellBounds.yMin, 1);
    }


    // return true if we should place a back tile for a light
    bool DetermineBackTile(int x, int y)
    {
        // how many adjacent tiles have a back tile
        int adjacent = 0;

        Vector2Int[] checkDirections = new Vector2Int[4];

        checkDirections[0] = new Vector2Int(x + 1, y);
        checkDirections[1] = new Vector2Int(x - 1, y);
        checkDirections[2] = new Vector2Int(x, y + 1);
        checkDirections[3] = new Vector2Int(x, y - 1);

        for (int i = 0; i < 4; i++)
        {
            if (frontTemplate.GetPixel(checkDirections[i].x, checkDirections[i].y) == backLayerColor)
                adjacent++;
        }

        if (adjacent >= 2) return true;
        else return false;

    }



}
