using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemSlotUI
{
    public string name;
    public Image itemIcon;
    public TMPro.TextMeshProUGUI quatityText;

    public void Set(Sprite icon, int quatity, string nameItem)
    {
        itemIcon.color = Color.white;
        name = nameItem;
        if (itemIcon != null) itemIcon.sprite = icon;
        if (quatityText != null) quatityText.text = quatity.ToString();
    }
}

public class UIManager : SingletonPersistentTemplate<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("HUD Elements")]
    [SerializeField] private StatBarUI playerHealthBar;
    [SerializeField] private StatBarUI playerStaminahBar;
    [SerializeField] private ItemSlotUI[] weaponSlots = new ItemSlotUI[2];
    [SerializeField] private ItemSlotUI[] itemSlots   = new ItemSlotUI[2];

    [Header("Level Clear Elements")]
    [SerializeField] private TMPro.TextMeshProUGUI levelClearScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI timeBonusText;
    [SerializeField] private TMPro.TextMeshProUGUI noDeathBonusText;
    [SerializeField] private TMPro.TextMeshProUGUI upgradePointsText;
    [SerializeField] private GameObject noDeathBonusRow;

    [Header("Game Over Elements")]
    [SerializeField] private TMPro.TextMeshProUGUI finalScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI runsPlayedText;

    [Header("In-Game Elements")]
    [SerializeField] private GameObject    pickupCanva;
    [SerializeField] private RectTransform pickupWindowRect;
    [SerializeField] private Text          pickupPrompt;
    [SerializeField] private float panelAnimationSpeed = 1.0f;
    [SerializeField] private float panelAnimDuration   = 1.0f;

    [Header("Transition")]
    [SerializeField] private CanvasGroup fadeOverlay;   // full-screen black fade

    private Coroutine pickupPanelAnimation;

    private void Start()
    {
        if (Player.Instance != null &&
            Player.Instance.Core.TryGetCoreComponent(out Stats stats))
        {
            playerHealthBar.SetTarget(stats.Health.MaxValue, stats.Health.MaxValue);
            playerStaminahBar.SetTarget(stats.Stamina.MaxValue, stats.Stamina.MaxValue);
        }

        foreach (ItemSlotUI slot in itemSlots)
        {
            slot.itemIcon.sprite = null;
            slot.itemIcon.color = new Color(0, 0, 0, 0);
            slot.quatityText.text = "";
        }
    }

    public void UpdateItemSlot(ItemData itemData, int quatity)
    {
        int emptySlotIndex = -1;
        int numEmptySlot = 0;

        foreach(ItemSlotUI slotUI in itemSlots)
        {
            if (slotUI.name == itemData.itemName)
            {
                slotUI.quatityText.text = quatity.ToString();
                return;
            }

            if (slotUI.name.Length == 0) numEmptySlot++;
            emptySlotIndex++;
        }

        emptySlotIndex = numEmptySlot == itemSlots.Length ? 0 : emptySlotIndex;
        itemSlots[emptySlotIndex].Set(itemData.itemImage, quatity, itemData.itemName);
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

    public void SetPickupPanel(Vector2 position, string prompt)
    {
        pickupCanva.SetActive(true);
        pickupCanva.transform.position = position;
        pickupPrompt.text = prompt;
        pickupPanelAnimation = StartCoroutine(PanelAnimation(pickupWindowRect, RectTransform.Axis.Horizontal));
    }

    public void HidePickupPanel()
    {
        pickupCanva.SetActive(false);
        StopCoroutine(pickupPanelAnimation);
    }

    IEnumerator PanelAnimation(RectTransform panelTransform, RectTransform.Axis axis)
    {
        Vector2 originalSize = panelTransform.rect.size;
        float progress = 0f;
        while (progress < panelAnimDuration)
        {
            progress += Time.deltaTime * panelAnimationSpeed;
            float currentSize = Mathf.Lerp(0.0f, axis == RectTransform.Axis.Horizontal ? 
                                originalSize.x : originalSize.y, 
                                progress / panelAnimDuration);
            panelTransform.SetSizeWithCurrentAnchors(axis, currentSize);
            yield return null;
        }

        yield return null;
    }
}
