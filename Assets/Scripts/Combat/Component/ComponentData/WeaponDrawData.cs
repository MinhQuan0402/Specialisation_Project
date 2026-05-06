public class WeaponDrawData : ComponentData<AttackDraw>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponDraw);
    }
}