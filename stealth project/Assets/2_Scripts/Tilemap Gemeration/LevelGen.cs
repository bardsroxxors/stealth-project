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
        lightsContainer.transform.parent = frontMap.transform;

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
}
