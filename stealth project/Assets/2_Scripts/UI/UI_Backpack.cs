using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UI_Backpack : MonoBehaviour
{
    public bool open = false;
    public int currentSelection = 0;
    public bool selectorVisible = false;
    public float selectorXPos = 0.2f;
    public int maxSlots = 4;
    public GameObject slotPrefab;
    public float slotSpacing = 1f;
    public float bottomPosition = 0.55f;
    public float bottomSpacing = 0.55f;
    public float topSpacing = 0.55f;
    public GameObject[] slots;
    public int numberHeld = 0;
    public GameObject selector;
    
    //public GameObject top;
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


    public bool AddItem(GameObject item)
    {
        if (numberHeld >= maxSlots) return false;

        int index = maxSlots - numberHeld -1;
        SpriteRenderer slotSprite = slots[index].transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (slotSprite) slotSprite.sprite = item.GetComponent<SpriteRenderer>().sprite;

        item.transform.parent = slots[index].transform;
        item.transform.localPosition = Vector3.zero;
        item.SendMessage("PickedUp");
        
        numberHeld++;

        return true;
    }

    public void MoveSelection(int direction)
    {
        if(!selector.active)
        {
            selector.SetActive(true);
            if (direction == 1) currentSelection = slots.Length -1;
            else if (direction == -1) currentSelection = 0;
        }

        else
        {
            if(direction == 1 && currentSelection == slots.Length - 1) currentSelection = 0;
            else if (direction == -1 && currentSelection == 0) currentSelection = slots.Length - 1;
        }
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
            selector.SetActive(false);
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

        //pos.y += topSpacing;
        //top.transform.position = pos;

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

