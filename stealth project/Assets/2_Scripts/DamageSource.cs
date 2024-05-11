using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public int damageAmount = 1;
    public List<string> applyToTags = new List<string>();

    public bool hasHit = false;



    public void RefreshDamageSource()
    {
        hasHit = false;
    }


}
