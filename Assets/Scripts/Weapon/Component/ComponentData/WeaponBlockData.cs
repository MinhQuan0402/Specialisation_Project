using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponBlockData : ComponentData<AttackBlock>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponBlock);
    }

    public override void PopulateStatUI(RectTransform parentContent)
    {
        float damageDuctionPer = GetAttackData(0).BlockDirectionalInformation[0].DamageAbsorption * 100.0f;
        UIManager.Instance.CreateWeaponStatUI(parentContent,
            new TextUIInput("Damage Reduction: ", Color.black),
            new TextUIInput(damageDuctionPer.ToString(), Color.red));
    }
}