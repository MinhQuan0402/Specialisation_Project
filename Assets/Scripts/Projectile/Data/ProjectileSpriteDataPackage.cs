using System;
using UnityEngine;

[Serializable]
public class ProjectileSpriteDataPackage : ProjectileDataPackage
{
    [field: SerializeField] public Sprite Sprite { get; private set; }
}