using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UI_Backpack : MonoBehaviour
{
    public bool open = false;
    public int maxSlots = 4;
    public GameObject slotPrefab;
    public float slotSpacing = 1f;
    public float bottomPosition = 0.55f;
    public float bottomSpacing = 0.55f;
    public float topSpacing = 0.55f;
    public GameObject[] slots;
    
    public GameObject top;
    public GameObject bottom;

    // Start is called before the first frame update
    void Start()
    {
        slots = new GameObject[maxSlots];
        PositionSlots();
        ToggleOpen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleOpen()
    {
        if (open)
        {
            int nbChildren = transform.childCount;
            if (nbChildren > 0)
            {
                for (int i = nbChildren - 1; i >= 0; i--)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            open = false;
        }

        else
        {
            int nbChildren = transform.childCount;
            if (nbChildren > 0)
            {
                for (int i = nbChildren - 1; i >= 0; i--)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            open = true;
        }
    }

    public void PositionSlots()
    {
        // delete existing navPoint objects
        int nbChildren = transform.childCount;
        if (nbChildren > 0)
        {
            for (int i = nbChildren - 1; i >= 0; i--)
            {
                if(transform.GetChild(i).name.Contains("bag slot"))
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
                
            }
        }

        slots = new GameObject[maxSlots];

        Vector3 pos = transform.position;
        pos.y += bottomPosition;

        bottom.transform.position = pos;
        pos.y += bottomSpacing;

        for (int i = 0; i < slots.Length; i++)
        {
            if(i != 0)
                pos.y += slotSpacing;
            slots[i] = Instantiate(slotPrefab, pos, Quaternion.identity, transform);
        }

        pos.y += topSpacing;
        top.transform.position = pos;

    }


}







[CustomEditor(typeof(UI_Backpack))]


public class UI_BackpackEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UI_Backpack generator = (UI_Backpack)target;


        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            generator.PositionSlots();
        }

        EditorGUILayout.Space();

    }

}

