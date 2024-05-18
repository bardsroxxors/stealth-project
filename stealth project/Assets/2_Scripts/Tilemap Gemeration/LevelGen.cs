using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGen : MonoBehaviour
{

    public Tilemap tilemap;
    public TileBase tile;
    public Texture2D template;
    public Color wallColor;


    public void Generate()
    {

        tilemap.ClearAllTiles();

        for (int y = 0; y < (int)template.height; y++)
        {


            // for each column in row
            for (int x = 0; x < (int)template.width; x++)
            {
                if (template.GetPixel(x, y) == wallColor)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }

            }

        }
    }
}
