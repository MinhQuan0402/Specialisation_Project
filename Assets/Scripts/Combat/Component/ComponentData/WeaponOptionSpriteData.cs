public class WeaponOptionSpriteData : ComponentData<AttackOptionalSprite>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponOptionalSprite);
    }
}