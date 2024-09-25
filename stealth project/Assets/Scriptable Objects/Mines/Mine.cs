using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Object/Mine")]

public class Mine : ScriptableObject
{
    public Sprite disarmedSprite;
    public Sprite armedSprite;
    public GameObject payload;
    public float armTime = 1f;
    public bool directEffect = false;
    public e_EnemyConditions effect;
    public float duration = 0f;
    public bool sticky = false;
    //public float triggerRadius = 1f;
    public string[] applyToTags;
}
