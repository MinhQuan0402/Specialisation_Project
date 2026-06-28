using UnityEngine;

[System.Serializable]
public class WeaponProjectileSpawnerData : ComponentData<AttackProjectileSpawner>
{
    protected override void SetComponentDenpendency()
    {
        ComponentDependency = typeof(WeaponProjectileSpawner);
    }

    public override void PopulateStatUI(RectTransform parentContent)
    {
        UIManager.Instance.CreateWeaponStatUI(parentContent,
            new TextUIInput("Base Damage: ", Color.black),
            new TextUIInput(GetAttackData(0).ProjectileSpawnInfo[0].DamageData.Amount.ToString(), Color.red));

        float crit_chance = GetAttackData(0).ProjectileSpawnInfo[0].DamageData.CritChance * 100.0f;
        UIManager.Instance.CreateWeaponStatUI(parentContent,
            new TextUIInput("Crit Chance: ", Color.black),
            new TextUIInput(crit_chance.ToString() + "%", Color.red));

        float crit_multiplier = GetAttackData(0).ProjectileSpawnInfo[0].DamageData.CritMultiplier;
        UIManager.Instance.CreateWeaponStatUI(parentContent,
            new TextUIInput("Crit Multiplier: ", Color.black),
            new TextUIInput("x" + crit_multiplier.ToString() + "%", Color.red));
    }
}