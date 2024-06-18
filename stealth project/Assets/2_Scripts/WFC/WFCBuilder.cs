using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WFCBuilder : MonoBehaviour
{
    public float timer = 0.2f;
    private float t_timer;
    public int width;
    public int height;

    // a list containing all possible nodes
    public List<WFCNode> allNodes = new List<WFCNode>();



    // a 2d array for storing our collapsed nodes
    private WFCNode[,] _finishedGrid;

    // a 2d array for storing data about the entropy of each coordinate
    // meaning how many possible things that coordinate can be
    private List<WFCNode>[,] _possibleGrid;

    

    // a list to store tile positions that need collapsing
    private List<Vector2Int> _toCollapse = new List<Vector2Int>();

    private Vector2Int[] gridOffsets = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // top
        new Vector2Int(0, -1),  // bottom
        new Vector2Int(1, 0),   // right
        new Vector2Int(-1, 0)   // left
    };


    private void Start()
    {
        



        CollapseWorld();

    }

    private void CollapseWorld()
    {
        // initialise entropy grid
        _finishedGrid = new WFCNode[width, height];
        _possibleGrid = new List<WFCNode>[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _possibleGrid[x, y] = new List<WFCNode>(allNodes);
            }
        }

        Vector2Int nextNode = new Vector2Int(width/2, height/2);

        _possibleGrid[nextNode.x, nextNode.y] = new List<WFCNode>();
        _finishedGrid[nextNode.x, nextNode.y] = allNodes[Random.Range(0, allNodes.Count)];

        Vector3 pos = new Vector3(nextNode.x, nextNode.y, 0);
        Instantiate(_finishedGrid[nextNode.x, nextNode.y].prefab, pos, Quaternion.identity, this.transform);

        ReduceNeighbours(nextNode);

        
    }

    private void Update()
    {

        if (Input.GetButtonDown("Jump"))
        {
            int nbChildren = transform.childCount;
            if (nbChildren > 0)
            {
                for (int i = nbChildren - 1; i >= 0; i--)
                {

                    DestroyImmediate(transform.GetChild(i).gameObject);
                    
                }
            }
            CollapseWorld();
        }

        t_timer -= Time.deltaTime;
        int nodesLeft = (width * height);

        //while (nodesLeft > 0)
        if (t_timer <= 0 && nodesLeft > 0)
        {
            t_timer = timer;

            Vector2Int nextNode = GetLeastEntropy();

            //Debug.Log(nextNode);


            if (IsInsideGrid(nextNode))
            {
                //Debug.Log(_possibleGrid[nextNode.x, nextNode.y]);
                _finishedGrid[nextNode.x, nextNode.y] = _possibleGrid[nextNode.x, nextNode.y][Random.Range(0, allNodes.Count)];
                _possibleGrid[nextNode.x, nextNode.y] = new List<WFCNode>();

                Vector3 pos = new Vector3(nextNode.x, nextNode.y, 0);
                Instantiate(_finishedGrid[nextNode.x, nextNode.y].prefab, pos, Quaternion.identity, this.transform);
                //Debug.Log("placing guy with name " + _finishedGrid[nextNode.x, nextNode.y].name);
                ReduceNeighbours(nextNode);

                nodesLeft--;
            }



        }
    }



    // Iterates through the uncollapsed nodes (_possibleGrid)
    // and gets one that has the lowest or tied lowest possible tiles
    private Vector2Int GetLeastEntropy()
    {
        List<Vector2Int> lowest = new List<Vector2Int>();
        int currentLowest = 1000;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_possibleGrid[x, y].Count < currentLowest && _possibleGrid[x, y].Count > 0)
                {
                    if(IsInsideGrid(new Vector2Int(x, y)))
                    {
                        currentLowest = _possibleGrid[x, y].Count;
                        lowest = new List<Vector2Int>();
                        lowest.Add(new Vector2Int(x, y));
                    }
                    
                }
                else if (_possibleGrid[x, y].Count == currentLowest && _possibleGrid[x, y].Count > 0)
                {
                    if (IsInsideGrid(new Vector2Int(x, y)))
                    {
                        lowest.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        if (lowest.Count > 0)
        {
            return lowest[Random.Range(0, lowest.Count)];
        }


        Debug.Log("Failed to find a lowest entropy value");
        return Vector2Int.zero;
    }


    // Iterate through the neighbours of a tile
    // Use the edge socket of the tile to remove possible tiles from the uncollapsed neighbour
    private void ReduceNeighbours(Vector2Int pos)
    {
        // Iterate through directions for neighbours
        for(int i = 0; i < 4; i++)
        {
            Vector2Int npos = pos + gridOffsets[i];
            if (IsInsideGrid(npos) && _possibleGrid[npos.x,npos.y].Count > 0){
                // Got valid neighbour

                // Get socket we are comparing to this neighbour
                List<TileSocket> sockets = new List<TileSocket>();// = TileSocket.none;
                switch (i) {
                    case 0: // top
                        sockets = _finishedGrid[pos.x, pos.y].Top;
                        break;
                    case 1: // bottom
                        sockets = _finishedGrid[pos.x, pos.y].Bottom;
                        break;
                    case 2: // right
                        sockets = _finishedGrid[pos.x, pos.y].Right;
                        break;
                    case 3: // left
                        sockets = _finishedGrid[pos.x, pos.y].Left;
                        break;
                }

                // Use that socket to compare to the neighbour's list of possible tiles
                if(sockets.Count > 0)
                {
                    // Iterate through the list of possible tiles for that neighbour
                    for (int n = _possibleGrid[npos.x, npos.y].Count -1; n >= 0; n--)
                    {
                        // Get the correct socket from the neighbour's possible tile
                        List<TileSocket> nSockets = new List<TileSocket>();
                        switch (i)
                        {
                            case 0: // top
                                nSockets = _possibleGrid[npos.x, npos.y][n].Bottom;
                                break;
                            case 1: // bottom
                                nSockets = _possibleGrid[npos.x, npos.y][n].Top;
                                break;
                            case 2: // right
                                nSockets = _possibleGrid[npos.x, npos.y][n].Left;
                                break;
                            case 3: // left
                                nSockets = _possibleGrid[npos.x, npos.y][n].Right;
                                break;
                        }


                        bool match = false;
                        foreach (TileSocket socket in nSockets)
                        {
                            if(sockets.Contains(socket))
                                match = true;
                        }
                        // Check if the two socket lists contain a common socket
                        if(!match)
                        {
                            // If they don't match remove that tile from the neighbour's list
                            _possibleGrid[npos.x, npos.y].RemoveAt(n);
                        }
                    }
                    
                }
            }
        }
    }

    // compares list of potential nodes to list of valid nodes and removes the invalid ones from potentials list
    private void WhittleNodes(List<WFCNode> potentialNodes, List<WFCNode> validNodes)
    {

    }

    private bool IsInsideGrid(Vector2Int coord)
    {
        if (coord.x >= 0 && coord.x < width && coord.y >= 0 && coord.y < height)
            return true;
        
        else return false;
    }
}
