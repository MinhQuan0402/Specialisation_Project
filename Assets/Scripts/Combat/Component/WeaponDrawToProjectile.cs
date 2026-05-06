public class WeaponDrawToProjectile : WeaponComponent
{
    private WeaponDraw draw;
    private WeaponProjectileSpawner projectileSpawner;

    private readonly ProjectileDrawModifierDataPackage drawModifierDataPackage = new ProjectileDrawModifierDataPackage();
    
    private void HandleEvaluateCurve(float value)
    {
        drawModifierDataPackage.DrawPercentage = value;
    }

    private void HandleSpawnProjectile(Projectile projectile)
    {
        projectile.SendDataPackage(drawModifierDataPackage);
    }

    protected override void HandleEnter()
    {
        drawModifierDataPackage.DrawPercentage = 0f;
    }
    
    protected override void Start()
    {
        base.Start();

        draw = GetComponent<WeaponDraw>();
        projectileSpawner = GetComponent<WeaponProjectileSpawner>();

        draw.OnEvaluateCurve += HandleEvaluateCurve;
        projectileSpawner.OnSpawnProjectile += HandleSpawnProjectile;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        draw.OnEvaluateCurve -= HandleEvaluateCurve;
        projectileSpawner.OnSpawnProjectile -= HandleSpawnProjectile;
    }
}