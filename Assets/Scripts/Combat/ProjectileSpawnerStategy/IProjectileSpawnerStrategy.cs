using System;
using UnityEngine;

public interface IProjectileSpawnerStrategy
{
    void ExecuteSpawnStrategy(ProjectileSpawnInfo projectileSpawnInfo, Vector3 spawnerPos, int facingDirection,
        ObjectPools objectPools, Action<Projectile> OnSpawnProjectile);
}