[System.Serializable]
public class WeaponTargeterData : ComponentData<AttackTargeter>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponTargeter);
    }
}