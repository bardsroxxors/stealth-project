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

            generator.Generate(generator.map);
            Debug.Log("generate!");

        }

        EditorGUILayout.Space();

    }

}