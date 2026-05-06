public class WeaponChargeData : ComponentData<AttackCharge>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponCharge);
    }
}