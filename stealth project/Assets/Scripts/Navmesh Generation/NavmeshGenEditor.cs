using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(NavmeshGenerator))]


public class NavmeshGenEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        NavmeshGenerator generator = (NavmeshGenerator)target;


        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            Debug.Log("Generate!");
            generator.Generate();
            Debug.Log("Done!");

        }

        EditorGUILayout.Space();

    }

}