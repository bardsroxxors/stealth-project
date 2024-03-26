using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LineRendererSmoother))]
public class LineRendererSmootherEditor : Editor
{
    private LineRendererSmoother lineSmoother;

    private SerializedProperty line;
    private SerializedProperty initialState;
    private SerializedProperty smoothingLength;
    private SerializedProperty smoothingSections;

    private GUIContent updateInitialStateGUIContent = new GUIContent("Set Initial State");
    private GUIContent smoothButtonGUIContent = new GUIContent("Smooth Path");
    private GUIContent restoreDefaultGUIContent = new GUIContent("Restore Default State");

    private bool expandCurves = false;
    private BezierCurve[] curves;

    private void OnEnable()
    {
        lineSmoother = (LineRendererSmoother)target;

        if(lineSmoother.lineRenderer != null)
        {
            lineSmoother.lineRenderer = lineSmoother.GetComponent<LineRenderer>();
        }
        line = serializedObject.FindProperty("lineRenderer");
        initialState = serializedObject.FindProperty("initialState");
        smoothingLength = serializedObject.FindProperty("smoothingLength");
        smoothingSections = serializedObject.FindProperty("smoothingSections");

        curves = new BezierCurve[lineSmoother.lineRenderer.positionCount - 1];
    }

    public override void OnInspectorGUI()
    {
        if (lineSmoother == null)
            return;

        EditorGUILayout.PropertyField(line);
        EditorGUILayout.PropertyField(initialState);
        EditorGUILayout.PropertyField(smoothingLength);
        EditorGUILayout.PropertyField(smoothingSections);

        if (GUILayout.Button(updateInitialStateGUIContent))
        {
            lineSmoother.initialState = new Vector3[lineSmoother.lineRenderer.positionCount];
            lineSmoother.lineRenderer.GetPositions(lineSmoother.initialState);
        }

        EditorGUILayout.BeginHorizontal();
        {
            GUI.enabled = lineSmoother.lineRenderer.positionCount >= 3;
            if (GUILayout.Button(smoothButtonGUIContent))
            {
                SmoothPath();
            }

            bool lineAltered = lineSmoother.lineRenderer.positionCount == lineSmoother.initialState.Length;

            if (lineAltered)
            {
                Vector3[] positions = new Vector3[lineSmoother.lineRenderer.positionCount];
                lineSmoother.lineRenderer.GetPositions(positions);

                // this function compares each element of the list to each other to see if the lists are equal
                lineAltered = positions.SequenceEqual(lineSmoother.initialState);
            }

            GUI.enabled = lineAltered;
            if (GUILayout.Button(restoreDefaultGUIContent))
            {
                lineSmoother.lineRenderer.positionCount = lineSmoother.initialState.Length;
                lineSmoother.lineRenderer.SetPositions(lineSmoother.initialState);

                if(curves.Length != lineSmoother.lineRenderer.positionCount - 1)
                {
                    curves = new BezierCurve[lineSmoother.lineRenderer.positionCount - 1];
                }
            }
        
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();


    }


    private void SmoothPath()
    {

    }

}
