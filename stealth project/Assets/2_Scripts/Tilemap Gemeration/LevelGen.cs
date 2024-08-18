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
    public Color hideColor;
    public Color enemyColor;
    public Color doorColor;
    public Color hiddenColor;
    public Color backLayerColor;
    public GameObject lightsContainer;
    public GameObject hidesContainer;
    public GameObject doorsContainer;
    public GameObject hatchesContainer;
    public GameObject enemiesContainer;
    public GameObject hangingLight;
    public GameObject standingLight;
    public GameObject hideObject;
    public GameObject hatchObject;
    public GameObject doorObject;
    public GameObject enemyObject;
    public Color backdropColor;
    public GameObject backdropPlane;


    public void Generate()
    {

        frontMap.ClearAllTiles();
        backMap.ClearAllTiles();
        //DestroyImmediate(lightsContainer);


        SetupContainer(lightsContainer, "Lights");
        SetupContainer(doorsContainer, "Doors");
        SetupContainer(hidesContainer, "Hides");
        SetupContainer(enemiesContainer, "Enemies");
        SetupContainer(hatchesContainer, "Hatches");

        /*

        // Setup lights object
        lightsContainer = GameObject.Find("Lights");
        if(lightsContainer == null)
        {
            lightsContainer = new GameObject("Lights");
            lightsContainer.transform.parent = this.transform;
        }

        for(int i = lightsContainer.transform.childCount -1; i >= 0; i--)
        {
            DestroyImmediate(lightsContainer.transform.GetChild(i).gameObject);
        }

        // Setup hides object
        hidesContainer = GameObject.Find("Hides");
        if (hidesContainer == null)
        {
            hidesContainer = new GameObject("Hides");
            hidesContainer.transform.parent = this.transform;
        }

        for (int i = hidesContainer.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(hidesContainer.transform.GetChild(i).gameObject);
        }*/





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
                
                // Doors
                else if (frontTemplate.GetPixel(x, y) == doorColor)
                {
                    if (frontTemplate.GetPixel(x, y + 1) == doorColor)
                    {
                        //Debug.Log("Door");
                        Instantiate(doorObject,
                                    frontMap.GetCellCenterWorld(new Vector3Int(x, y, 0)) + new Vector3(-0.5f,0.5f,0),
                                    Quaternion.identity,
                                    doorsContainer.transform);
                    }


                    // Hatches
                    else if (frontTemplate.GetPixel(x, y + 1) != doorColor && frontTemplate.GetPixel(x, y - 1) != doorColor)
                    {
                        Instantiate(hatchObject,
                                    frontMap.GetCellCenterWorld(new Vector3Int(x, y, 0)) + new Vector3(0, 0, 0),
                                    Quaternion.identity,
                                    hatchesContainer.transform);
                    }

                }



                // Enemies
                else if (frontTemplate.GetPixel(x, y) == enemyColor)
                {
                    if (frontTemplate.GetPixel(x, y + 1) == enemyColor)
                    {
                        //Debug.Log("Door");
                        Instantiate(enemyObject,
                                    frontMap.GetCellCenterWorld(new Vector3Int(x, y, 0)) + new Vector3(-0.5f, 0.15f, 0),
                                    Quaternion.identity,
                                    enemiesContainer.transform);
                    }

                }

                // Hides
                else if (frontTemplate.GetPixel(x, y) == hideColor)
                {
                    //Debug.Log("Hide");
                    Instantiate(hideObject,
                                frontMap.GetCellCenterWorld(new Vector3Int(x, y, 0)),
                                Quaternion.identity,
                                hidesContainer.transform);
                    

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



    void SetupContainer(GameObject container, string name)
    {
        container = GameObject.Find(name);

        if (container == null)
        {
            container = new GameObject(name);
            container.transform.parent = this.transform;
        }

        for (int i = container.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(container.transform.GetChild(i).gameObject);
        }
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
