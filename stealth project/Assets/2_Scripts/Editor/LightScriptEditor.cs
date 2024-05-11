using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(LightScript))]

public class LightScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LightScript light = (LightScript)target;


        EditorGUILayout.Space();

        if (GUILayout.Button("Generate collider"))
        {
            Debug.Log("Generate!");
            light.SetCollider();
            Debug.Log("Done!");

        }

        EditorGUILayout.Space();

    }
}
