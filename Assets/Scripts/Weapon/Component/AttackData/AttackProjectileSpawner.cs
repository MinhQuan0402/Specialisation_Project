using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AttackProjectileSpawner : AttackData
{
    [field: SerializeField] public ProjectileSpawnInfo[] ProjectileSpawnInfo { get; private set;}
}

[Serializable]
public struct ProjectileSpawnInfo
{
    [field: SerializeField] public Vector2 Offset { get; private set; }
    [field: SerializeField] public Vector2 Direction { get; private set; }
    [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
    [field: SerializeField] public ProjectileDamageDataPackage DamageData { get; private set; }
    [field: FormerlySerializedAs("<KnockbackData>k__BackingField")] [field: SerializeField] public ProjectileKnockBackDataPackage KnockBackData { get; private set; }
    [field: SerializeField] public ProjectilePoiseDamagePackage PoiseDamageData { get; private set; }
    [field: SerializeField] public ProjectileSpriteDataPackage SpriteData { get; private set; }
}