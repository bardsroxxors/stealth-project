using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Object/Equipment Register")]

public class SO_EquipRegister : ScriptableObject
{
    public List<e_Equipment> enums;
    public List<SO_Equipment> equipment;


    public SO_Equipment GetEquipment(int index)
    {
        return equipment[index];
    }

    public SO_Equipment GetEquipment(e_Equipment e)
    {
        if (enums.Contains(e)) 
        {
            for (int i = 0; i< enums.Count; i++) 
            {
                if (enums[i] == e)
                    return equipment[i];
            }
        }
        return null;
    }

}
