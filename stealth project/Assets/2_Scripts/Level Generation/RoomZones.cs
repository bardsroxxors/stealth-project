using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomZones : MonoBehaviour
{

    public Vector2Int minDimensions = Vector2Int.zero;
    public Vector2Int maxDimensions = Vector2Int.zero;

    // min and max distance to have between rooms
    public int minPadding = 10;
    public int maxPadding = 40;

    // size of the area to generate
    public Vector2Int area = Vector2Int.zero;

    // max number of tries to place a room 
    // before we decide we're finished
    public int maxAttempts = 100;

    // this stores the information for our generated layout
    private int[] gen;
    // 0 means empty
    // 1 means room fill
    // 2 means the center point of a room



    public void Generate()
    {
        gen = new int[area.x * area.y];
        for (int i = 0; i < gen.Length; i++)
        {
            gen[i] = 0;
        }
        Debug.Log(gen[0]);
        int attempts = 0;

        while (attempts  < maxAttempts)
        {
            Vector2Int c = new Vector2Int(  Random.Range(0 + maxDimensions.x, area.x - maxDimensions.x) , 
                                            Random.Range(0 + maxDimensions.y, area.y - maxDimensions.y) );

            int x = Random.Range(minDimensions.x, maxDimensions.x);
            int y = Random.Range(minDimensions.y, maxDimensions.y);
            /*// make sure they're odd numbers
            if (x % 2 == 0) x ++;
            if (y % 2 == 0) y ++;*/

            if(!CheckOverlap(c, x, y))
            {

            }

            attempts++;
        }


        // pick a random spot to be the center
        // pick a random area
        // check if it fits
        // fill it in
    }


    private bool CheckOverlap(Vector2Int c, int x, int y)
    {
        for (int i = c.x-x/2; i < c.x+x/2; i++)
        {
            for (int n = c.y-y/2; n < c.y+y/2; n++)
            {
                if (gen[i + n * area.x] == 0)
                    return false;
            }
        }

        return true;
    }

    private void PlaceRoom(Vector2Int c, int x, int y)
    {
        for (int i = c.x-x/2; i < c.x+x/2; i++)
        {
            for (int n = c.y-y/2; n < c.y+y/2; n++)
            {
                gen[i + n * area.x] = 1;

            }
        }

    }

}




#if UNITY_EDITOR
[CustomEditor(typeof(RoomZones))]
public class RoomZonesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoomZones generator = (RoomZones)target;

        if (GUILayout.Button("\ngenerate\n"))
        {
            generator.Generate();
        }

        
        DrawDefaultInspector();
    }
}
#endif
