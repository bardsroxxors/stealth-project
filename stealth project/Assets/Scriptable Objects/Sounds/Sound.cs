using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Object/Sound")]

public class Sound : ScriptableObject
{


    public float lifetime = 0.15f;
    public float scaleAtDeath = 3f;
    public float initialScale = 2.5f;
    public float awarenessIncrease = 0.15f;

}
