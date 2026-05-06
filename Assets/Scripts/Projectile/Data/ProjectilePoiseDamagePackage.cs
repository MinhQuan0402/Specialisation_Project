using System;
using UnityEngine;

[Serializable]
public class ProjectilePoiseDamagePackage : ProjectileDataPackage
{
    [field: SerializeField] public float Amount { get; private set; }
}