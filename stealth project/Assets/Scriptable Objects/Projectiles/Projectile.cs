using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Scriptable Object/Projectile")]

public class Projectile : ScriptableObject
{
    public Sprite icon;
    public bool TransferRotation = false;
    public float speed = 2f;
    public float gravity = 10f;
    public GameObject payload;
    public Sprite sprite;
    public float hitRadius = 1f;
    public float maxFallSpeed = 20f;
    public LayerMask collisionMask;

}
