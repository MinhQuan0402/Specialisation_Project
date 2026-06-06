using UnityEngine;

public class UIManager : SingletonPersistentTemplate<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("HUD Elements")]
    [SerializeField] private StatBarUI playerHealthBar;
    [SerializeField] private StatBarUI playerStaminahBar;
    /*[SerializeField] private WeaponSlotUI weaponSlotA;
    [SerializeField] private WeaponSlotUI weaponSlotB;*/

    [Header("Level Clear Elements")]
    [SerializeField] private TMPro.TextMeshProUGUI levelClearScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI timeBonusText;
    [SerializeField] private TMPro.TextMeshProUGUI noDeathBonusText;
    [SerializeField] private TMPro.TextMeshProUGUI upgradePointsText;
    [SerializeField] private GameObject noDeathBonusRow;

    [Header("Game Over Elements")]
    [SerializeField] private TMPro.TextMeshProUGUI finalScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI runsPlayedText;

    [Header("Transition")]
    [SerializeField] private CanvasGroup fadeOverlay;   // full-screen black fade

    private void Start()
    {
        if (Player.Instance != null &&
            Player.Instance.Core.TryGetCoreComponent(out Stats stats))
        {
            playerHealthBar.SetTarget(stats.Health.MaxValue, stats.Health.MaxValue);
            playerStaminahBar.SetTarget(stats.Stamina.MaxValue, stats.Stamina.MaxValue);
        }
    }

    public void SetHealthBar(float value)  => playerHealthBar.SetInitValue(value);
    public void SetStaminaBar(float value) => playerStaminahBar.SetInitValue(value);

    public void Health_OnValueChanged(float _, float currValue, float maxValue)
    {
        playerHealthBar.SetTarget(currValue, maxValue);
    }

    public void Stamina_OnValueChanged(float _, float currValue, float maxValue)
    {
        playerStaminahBar.SetTarget(currValue, maxValue);
    }
}
