using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Object/Equipment")]

public class SO_Equipment : ScriptableObject
{
    public e_Equipment e_equip;
    public Sprite icon;
    public int maxCarry;
    public GameObject projectilePrefab;
    public Projectile projectileSO;


}
