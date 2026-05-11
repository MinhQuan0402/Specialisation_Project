public class WeaponChargeToProjectileSpawner : WeaponComponent<WeaponChargeToProjectileSpawnerData, AttackChargeToProjectileSpawner>
{
    private WeaponProjectileSpawner projectileSpawner;
    private WeaponCharge charge;
    
    private bool hasReadCharge;
    
    private ChargeProjectileSpawnerStrategy chargeProjectileSpawnerStrategy = new ChargeProjectileSpawnerStrategy();

    protected override void HandleEnter()
    {
        base.HandleEnter();
        hasReadCharge = false;
    }

    private void HandleCurrentInputChange(bool newInput)
    {
        if (newInput || hasReadCharge)
            return;
        
        chargeProjectileSpawnerStrategy.AngleVariation = currentAttackData.AngleVariation;
        chargeProjectileSpawnerStrategy.ChargeAmount = charge.TakeFinalChargeReading();
        
        projectileSpawner.SetProjectileSpawnerStrategy(chargeProjectileSpawnerStrategy);
        
        hasReadCharge = true;
    }

    protected override void Start()
    {
        base.Start();
        projectileSpawner = GetComponent<WeaponProjectileSpawner>();
        charge = GetComponent<WeaponCharge>();
        
        weapon.OnCurrentInputChange += HandleCurrentInputChange;
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        weapon.OnCurrentInputChange -= HandleCurrentInputChange;
    }
}