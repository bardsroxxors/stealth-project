using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Projectile : ScriptableObject
{


    public float speed = 2f;
    public float gravity = 10f;
    public GameObject payload;
    public Sprite sprite;
    public float hitRadius = 1f;
    public float maxFallSpeed = 20f;
    public LayerMask collisionMask;

}
