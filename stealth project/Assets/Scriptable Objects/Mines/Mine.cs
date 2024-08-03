using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Mine : ScriptableObject
{
    public Sprite sprite;
    public GameObject payload;
    public bool directEffect = false;
    public e_EnemyConditions effect;
    public float duration = 0f;
    public bool sticky = false;
    //public float triggerRadius = 1f;
    public string[] applyToTags;
}
