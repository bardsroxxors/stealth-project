using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Object/Cloud")]

public class Cloud : ScriptableObject
{
    public Sprite sprite;
    public float lifespan = 5f;
    public bool blocksLight = false;
    public bool directEffect = false;
    public e_EnemyConditions effect;
    public float duration = 0f;
    public float radius = 1f;
    public string[] applyToTags;
}
