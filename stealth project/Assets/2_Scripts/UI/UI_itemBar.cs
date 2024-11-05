using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// this class handles the UI stuff for the item bar.

// Maybe I can use an event listener instead of having the PC directly tell this thing whenever they use an item or whatever
// whenever the active equip is changed
// when an item is used or picked back up, to update the remaining amount

public class UI_itemBar : MonoBehaviour
{
    public GameObject equipIndicator;
    public List<GameObject> equipSlotIcons;
    public List<GameObject> equipSlotNumbers;

    public List<e_Equipment> equipment;

    public SO_EquipRegister EquipRegister;

    public List<float> indicatorPositions;



    // Start is called before the first frame update
    void Start()
    {
        /*
        for (int i = 0; i < equipSlotIcons.Count; i++)
        {
            SetIcon(i, e_Equipment.empty);
        }*/

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetIndicator(int index)
    {
        equipIndicator.transform.localPosition = new Vector3(equipIndicator.transform.localPosition.x,
                                                            indicatorPositions[index],
                                                            equipIndicator.transform.localPosition.z);
    }



    public void SetIcon(int index, e_Equipment equip)
    {
        if (EquipRegister.GetEquipment(equip) != null)
        {
            equipSlotIcons[index].GetComponent<UnityEngine.UI.Image>().enabled = true;
            equipSlotIcons[index].GetComponent<UnityEngine.UI.Image>().sprite = EquipRegister.GetEquipment(equip).icon;
        }
            
        else
            equipSlotIcons[index].GetComponent<UnityEngine.UI.Image>().enabled = false;
    }
}
