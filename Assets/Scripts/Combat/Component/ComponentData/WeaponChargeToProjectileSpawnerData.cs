[System.Serializable]
public class WeaponChargeToProjectileSpawnerData : ComponentData<AttackChargeToProjectileSpawner>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponChargeToProjectileSpawner);
    }
}