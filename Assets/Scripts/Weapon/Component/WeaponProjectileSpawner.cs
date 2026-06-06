using System;
using UnityEngine;

public class WeaponProjectileSpawner : WeaponComponent<WeaponProjectileSpawnerData, AttackProjectileSpawner>
{
    public event Action<Projectile> OnSpawnProjectile;
    
    private CoreComp<Movement> movementCore;
    
    // Object pool to store projectiles so we don't have to keep instantiating new ones
    private readonly ObjectPools objectPools = new ObjectPools();

    // The strategy we use to spawn a projectile
    private IProjectileSpawnerStrategy projectileSpawnerStrategy;
    
    public void SetProjectileSpawnerStrategy(IProjectileSpawnerStrategy newStrategy)
    {
        projectileSpawnerStrategy = newStrategy;
    }
    
    private void HandleAttackAction()
    {
        if (Player.Instance.IsInteruptible) return;

        foreach (var projectileSpawnInfo in currentAttackData.ProjectileSpawnInfo)
        {
            // Spawn projectile based on the current strategy
            projectileSpawnerStrategy.ExecuteSpawnStrategy(projectileSpawnInfo, transform.position,
                movementCore.Comp.FacingDirection, objectPools, OnSpawnProjectile);
        }
    }
    
    private void SetDefaultProjectileSpawnStrategy()
    {
        // The default spawn strategy is the base ProjectileSpawnerStrategy class. It simply spawns one projectile based on the data per request
        projectileSpawnerStrategy = new ProjectileSpawnerStrategy();
    }
    
    protected override void HandleExit()
    {
        base.HandleExit();
            
        // Reset the spawner strategy every time the attack finishes in case some other component adjusted it
        SetDefaultProjectileSpawnStrategy();
    }
    
    protected override void Awake()
    {
        base.Awake();
            
        SetDefaultProjectileSpawnStrategy();
    }
    
    protected override void Start()
    {
        base.Start();

        movementCore ??= new CoreComp<Movement>(Core);

        eventHandler.OnAttackAction += HandleAttackAction;
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();

        eventHandler.OnAttackAction -= HandleAttackAction;
    }
    
    private void OnDrawGizmos()
    {
        if (data == null || !Application.isPlaying)
            return;

        foreach (var item in data.GetAllAttackData())
        {
            foreach (var point in item.ProjectileSpawnInfo)
            {
                var pos = transform.position + (Vector3)point.Offset;

                Gizmos.DrawWireSphere(pos, 0.2f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(pos, pos + (Vector3)point.Direction.normalized);
                Gizmos.color = Color.white;
            }
        }
    }
}