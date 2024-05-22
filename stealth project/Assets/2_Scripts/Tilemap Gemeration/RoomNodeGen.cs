using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeGen : MonoBehaviour
{
    // we will use this class to create a node graph

    // when we generate we choose

    
    [Range(0, 10)]
    public int width = 3;
    [Range(0, 10)]
    public int height = 2;
    [Range(1, 50)]
    public int maxRooms = 4;
    //public int minPerRow = 1;

    public RoomNode[,] rooms;


    public void Generate()
    {
        if (maxRooms > height * width)
        {
            Debug.Log("Invalid number of rooms");
            return;
        }

        rooms = new RoomNode[width, height];


        int spotsRemaining = (height * width) - maxRooms;
        while (spotsRemaining > 0)
        {
            int ex = Random.Range(0, width);
            int ey = Random.Range(0, height);

            if (rooms[ex, ey] == null)
            {
                rooms[ex, ey] = new RoomNode(true, new Vector2Int(ex,ey));
                spotsRemaining--;
            }
        }
        

        // create non-empty rooms
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(rooms[x, y] == null)
                {
                    rooms[x, y] = new RoomNode(false, new Vector2Int(x, y));
                }
                    
            }
        }

    }


    private void OnDrawGizmos()
    {
        if( rooms != null)
        {
            Handles.color = UnityEngine.Color.green;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!rooms[x, y].isEmpty)
                    {
                        Vector3 pos = new Vector3(x, y, 0);

                        Handles.DrawWireCube(pos, new Vector3(0.25f, 0.25f, 0.25f));
                    }


                    
                }
            }
        }
        


    }

}



// this is the class for a room node

public class RoomNode
{
    public RoomNode[] neighbours;
    public bool isEmpty = false;
    public Vector2Int graphPosition;


    public RoomNode(bool empty, Vector2Int graphPosition)
    {
        isEmpty = empty;
        this.graphPosition = graphPosition;
    }
}






[CustomEditor(typeof(RoomNodeGen))]
public class RoomNodeGenEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RoomNodeGen generator = (RoomNodeGen)target;


        EditorGUILayout.Space();

        if (GUILayout.Button("Create node graph!"))
        {
            generator.Generate();

        }

        EditorGUILayout.Space();

    }

}
