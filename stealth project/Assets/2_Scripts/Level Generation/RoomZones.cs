using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class RoomZones : MonoBehaviour
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
    // 2 means space buffer

    public bool drawGizmos = true;

    //public Training training;

    private bool[][] wave;



    public void Generate()
    {
        gen = new int[area.x * area.y];
        for (int i = 0; i < gen.Length; i++)
        {
            gen[i] = 0;
        }
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

                PlaceRoom(c, x, y);
            }
            else 
                attempts++;
        }

        TranslateToWave();

        Debug.Log("Done");
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
                if (gen[i + n * area.x] != 0)
                {

                    return true;
                }
                    
            }
        }
        
        return false;
    }

    private void PlaceRoom(Vector2Int c, int x, int y)
    {
        // place padding
        for (int i = c.x-x/2 - minPadding; i < c.x+x/2 + minPadding; i++)
        {
            for (int n = c.y-y/2 - minPadding; n < c.y+y/2 + minPadding; n++)
            {
                if (i >= 0 && i < area.x &&
                    n >= 0 && n < area.y)
                    gen[i + n * area.x] = 2;

            }
        }

        // place room
        for (int i = c.x-x/2; i < c.x+x/2; i++)
        {
            for (int n = c.y-y/2; n < c.y+y/2; n++)
            {
                gen[i + n * area.x] = 1;

            }
        }

        gen[c.x + c.y * area.x] = 3;
    }

    private void TranslateToWave()
    {
        wave = new bool[area.x * area.y][];

        // using our int[] data structure
        // create a bool[][] to pass into wfc

        // we have tiles[] from the training thing

        // from int[i] get tile

        // at bool[i]
        //      set all to false
        //      set bool[i][tile] to true
        /*
        for (int i = 0; i < gen.Length; i++)
        {
            wave[i] = new bool[training.tiles.Length];
            for (int t = 0; t < training.tiles.Length; t++)
            {
                wave[i][t] = false;
            }

            wave[i][ gen[i] ] = true;
        }*/
    }




    private void OnDrawGizmos()
    {
        if (drawGizmos == false)
        {
            return;
        }



        if (gen != null)
        {
            for (int x = 0; x < area.x; x++)
            {
                for (int y = 0; y < area.y; y++)
                {

                        Vector3 pos = transform.position;
                        pos.x += x;
                        pos.y += y;

                        if(gen[x + y * area.x] == 1)
                            Handles.color = UnityEngine.Color.green;
                        if (gen[x + y * area.x] == 2)
                            Handles.color = UnityEngine.Color.yellow;
                        if (gen[x + y * area.x] == 3)
                            Handles.color = UnityEngine.Color.red;
                        if (gen[x + y * area.x] == 0)
                            Handles.color = UnityEngine.Color.white;
                        

                        Handles.DrawWireCube(pos, new Vector3(0.25f, 0.25f, 0.25f));
                }
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
