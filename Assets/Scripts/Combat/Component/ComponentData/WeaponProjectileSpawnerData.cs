[System.Serializable]
public class WeaponProjectileSpawnerData : ComponentData<AttackProjectileSpawner>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponProjectileSpawner);
    }
}