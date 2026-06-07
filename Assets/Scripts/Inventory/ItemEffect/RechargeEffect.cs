using UnityEngine;

[CreateAssetMenu(fileName = "RechargeEffect", menuName = "Inventory/ItemEffects/RechargeEffect")]
public class RechargeEffect : ItemEffect
{
    [SerializeField] private float rechargePercent = 20.0f;

    public override bool Use(GameObject user)
    {
        Player player = Player.Instance;

        if (player == null)
        {
            Debug.LogError("Player component not found.");
            return false;
        }

        Stats playerStats = player.Core.GetComponent<Stats>();
        if (playerStats == null)
        {
            Debug.LogError("Stats component not found on the player.");
            return false;
        }

        if (playerStats.Stamina.CurrentValue >= playerStats.Stamina.MaxValue)
            return false;

        float amountToRecharge = playerStats.Stamina.MaxValue * (rechargePercent / 100.0f);
        playerStats.Stamina.Increase(amountToRecharge);

        return true;
    }
}