using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Object/Equipment Register")]

public class SO_EquipRegister : ScriptableObject
{
    public List<e_Equipment> enums;
    public List<Projectile> projectiles;


    public Projectile GetProjectile(int index)
    {
        return projectiles[index];
    }

    public Projectile GetProjectile(e_Equipment e)
    {
        if (enums.Contains(e)) 
        {
            for (int i = 0; i< enums.Count; i++) 
            {
                if (enums[i] == e)
                    return projectiles[i];
            }
        }
        return null;
    }

}
