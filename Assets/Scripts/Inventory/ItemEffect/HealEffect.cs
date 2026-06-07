using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Inventory/ItemEffects/HealEffect")]
public class HealEffect : ItemEffect
{
    [SerializeField] private float healPercent = 20.0f;

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

        if (playerStats.Health.CurrentValue >= playerStats.Health.MaxValue)
            return false;

        float amountToHealth = playerStats.Health.MaxValue * (healPercent / 100.0f);
        playerStats.Health.Increase(amountToHealth);

        return true;
    }
}
