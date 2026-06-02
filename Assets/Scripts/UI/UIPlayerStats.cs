using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Slider poiseBar;

    private Stats playerStats;
    private void Start()
    {
        playerStats = Player.Instance.Core.GetComponent<Stats>();

        healthBar.value = playerStats.Health.CurrentValue / playerStats.Health.MaxValue;
        staminaBar.value = playerStats.Stamina.CurrentValue / playerStats.Stamina.MaxValue;

        playerStats.Health.OnValueChanged += OnHealthChange;
        playerStats.Stamina.OnValueChanged += OnStaminaChange;
    }

    private void OnDestroy()
    {
        playerStats.Health.OnValueChanged -= OnHealthChange;
        playerStats.Stamina.OnValueChanged -= OnStaminaChange;
    }

    void OnHealthChange(float prevValue, float newValue)
    {
        healthBar.value = newValue / playerStats.Health.MaxValue;
    }

    void OnStaminaChange(float prevValue, float newValue)
    {
        staminaBar.value = newValue / playerStats.Stamina.MaxValue;
    }
}