using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(LevelGen))]


public class LevelGenEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelGen generator = (LevelGen)target;


        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            generator.Generate();

        }

        EditorGUILayout.Space();

    }

}