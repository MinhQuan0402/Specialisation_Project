using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] private Image healthBar;


    private Stats playerStats;
    private void Start()
    {
        playerStats = Player.Instance.Core.GetComponent<Stats>();
    }

    void Update()
    {
        if (healthBar.Equals(null)) return;
        
        // Assuming PlayerData is a class that holds the player's health data
        float healthPercentage = playerStats.Health.CurrentValue / playerStats.Health.MaxValue;
        healthBar.fillAmount = Mathf.Clamp01(healthPercentage);
    }
}
