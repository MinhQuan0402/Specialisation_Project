using System;
using UnityEngine;

[Serializable]
public class ProjectileDamageDataPackage : ProjectileDataPackage
{
    [field: SerializeField] public float Amount { get; private set; }
}