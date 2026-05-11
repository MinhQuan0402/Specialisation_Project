[System.Serializable]
public class WeaponTargeterToProjectileData : ComponentData
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponTargeterToProjectile);
    }
}