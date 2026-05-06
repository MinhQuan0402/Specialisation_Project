public class WeaponTargeterToProjectile : WeaponComponent
{
    private WeaponProjectileSpawner projectileSpawner;
    private WeaponTargeter targeter;
    
    private readonly ProjectileTargetsDataPackage targetsDataPackage = new ProjectileTargetsDataPackage();
    
    private void HandleSpawnProjectile(Projectile projectile)
    {
        targetsDataPackage.Targets = targeter.GetTargets();
        
        projectile.SendDataPackage(targetsDataPackage);
    }
    
    protected override void Start()
    {
        base.Start();

        projectileSpawner = GetComponent<WeaponProjectileSpawner>();
        targeter = GetComponent<WeaponTargeter>();

        projectileSpawner.OnSpawnProjectile += HandleSpawnProjectile;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
            
        projectileSpawner.OnSpawnProjectile -= HandleSpawnProjectile;
    }
}