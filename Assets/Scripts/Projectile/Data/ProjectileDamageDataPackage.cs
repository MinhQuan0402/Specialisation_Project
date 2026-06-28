using System;
using UnityEngine;

[Serializable]
public class ProjectileDamageDataPackage : ProjectileDataPackage
{
    [field: SerializeField] public float Amount { get; private set; }
    [field: SerializeField, Range(0f, 1f)] public float CritChance { get; private set; } = 0.15f;
    [field: SerializeField] public float CritMultiplier { get; private set; } = 2.0f;
}