[System.Serializable]
public class InputHoldData : ComponentData
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(InputHold);
    }
}