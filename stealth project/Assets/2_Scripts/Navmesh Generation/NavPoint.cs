using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NavpointType
{
    none,
    platform,
    leftEdge,
    rightEdge,
    solo
}

public class NavPoint : MonoBehaviour
{
    public Vector2          tileCoord = new Vector2 (0, 0);
    public int              platformIndex = -1;                  // used to group tiles onto platforms
    public NavpointType     type = NavpointType.none;
    public List<NavLink>    links = new List<NavLink>();
    public Vector3Int gridPosition;
}

public class NavLink
{
    public Vector3Int destinationCoords = new Vector3Int(0, 0, 0);
    public int linkScore = 1;
}
