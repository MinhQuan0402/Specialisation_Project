using System;
using UnityEngine;

[Serializable]
public class ProjectileKnockBackDataPackage : ProjectileDataPackage
{
    [field: SerializeField] public Vector2 Angle { get; private set; }
    [field: SerializeField] public float Strength { get; private set; }
}