using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DijkFOW : MonoBehaviour
{

    public Transform cameraTrans;
    public Tilemap tilemap;
    private Vector3Int centerTile;
    public GridLayout grid;
    public Vector2Int cameraGridSize = Vector2Int.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        centerTile = grid.WorldToCell(cameraTrans.position);


    }
}
