using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileSocket
{
    corner_edge,
    flat_a,
    flat_b,
    fill_edge,
    void_a,
    void_b,
    void_edge,
    T,
    L,
    none
}

//[CreateAssetMenu (fileName = "WFCNode", menuName = "WFC/Node")]
[System.Serializable]
public class WFCNode : MonoBehaviour
{
    public string name;
    //public GameObject prefab;
    //public Sprite sprite;
    public TileBase tile;

    

    public bool f_createRotationClones = false;
    public bool f_isRotationClone = false;


    public List<TileSocket> Top;
    public List<TileSocket> Bottom;
    public List<TileSocket> Left;
    public List<TileSocket> Right;


    public WFCNode(string name, GameObject prefab, bool f_createRotationClones, bool f_isRotationClone, 
        List<TileSocket> top, List<TileSocket> bottom, List<TileSocket> left, List<TileSocket> right)
    {
        this.name = name;
        //this.prefab = prefab;
        this.f_createRotationClones = f_createRotationClones;
        this.f_isRotationClone = f_isRotationClone;
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;

        
    }


    public void CreateClone()
    {
        /*
        WFCNode asset = ScriptableObject.CreateInstance<WFCNode>();

        AssetDatabase.CreateAsset(asset, "Assets/Scriptable Objects/WFC/"+name+"_clone.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;*/
    }

}


[CustomEditor(typeof(WFCNode))]
public class WFCNode_Editor :Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WFCNode node = (WFCNode)target;

        EditorGUILayout.Space();

        if (node.f_createRotationClones)
        {
            if (GUILayout.Button("\nCreate rotation clones\n"))
            {
                node.CreateClone();
            }

        }
        

        EditorGUILayout.Space();
    }
}



