using UnityEngine;

[CreateAssetMenu(fileName = "HealthPotionEffect", menuName = "Inventory/ItemEffects/HealthPotionEffect")]
public class HealthPotionEffect : ItemEffect
{
    [SerializeField] private float healthAmount = 20.0f;

    public override void Use(GameObject user)
    {
        Player player = Player.Instance;

        if (player == null)
        {
            Debug.LogError("Player component not found.");
            return;
        }

        Stats playerStats = player.Core.GetComponent<Stats>();
        if (playerStats == null)
        {
            Debug.LogError("Stats component not found on the player.");
            return;
        }
        if (playerStats.Health.CurrentValue < playerStats.Health.MaxValue)
        {
            // Restore health
            playerStats.Health.Increase(healthAmount);
        }
    }
}
