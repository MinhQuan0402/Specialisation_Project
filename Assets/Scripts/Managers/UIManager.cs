using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemSlotUI
{
    public string name;
    public Image itemIcon;
    public TMPro.TextMeshProUGUI quatityText;

    public void Set(string nameItem, Sprite icon, int quatity = -1)
    {
        itemIcon.color = Color.white;
        name = nameItem;
        if (itemIcon != null) itemIcon.sprite = icon;
        if (quatityText != null) quatityText.text = quatity.ToString();
    }

    public void Reset()
    {
        name = "";
        itemIcon.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        if (itemIcon != null) itemIcon.sprite = null;
        if (quatityText != null) quatityText.text = "";
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
    [SerializeField] private Text pickupPrompt;
    [SerializeField] private float panelAnimationSpeed = 1.0f;
    [SerializeField] private float panelAnimDuration   = 1.0f;

    [Header("Transition")]
    [SerializeField] private CanvasGroup fadeOverlay;   // full-screen black fade

    private Coroutine pickupPanelAnimation;
    private readonly Dictionary<RectTransform, Rect> rectSize = new();

    private void Start()
    {
        if (Player.Instance != null &&
            Player.Instance.Core.TryGetCoreComponent(out Stats stats))
        {
            playerHealthBar.SetTarget(stats.Health.MaxValue, stats.Health.MaxValue);
            playerStaminahBar.SetTarget(stats.Stamina.MaxValue, stats.Stamina.MaxValue);
        }

        foreach (ItemSlotUI slot in itemSlots) slot.Reset();
        foreach (ItemSlotUI slot in weaponSlots) slot.Reset();
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
                if (quatity == 0) slotUI.Reset();
                return;
            }

            if (slotUI.name.Length == 0) numEmptySlot++;
            emptySlotIndex++;
        }

        emptySlotIndex = numEmptySlot == itemSlots.Length ? 0 : emptySlotIndex;
        itemSlots[emptySlotIndex].Set(itemData.itemName, itemData.itemImage, quatity);
    }

    public void UpdateWeaponSlot(ItemData itemData, int index)
    {
        Debug.Log(index);
        if (index >= weaponSlots.Length || index < 0) return;
        if (itemData == null) return;

        weaponSlots[index].Set(itemData.itemName, itemData.itemImage);
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

    public void EnablePickupPanel(Vector2 position, string prompt)
    {
        pickupCanva.SetActive(true);
        pickupCanva.transform.position = position;
        pickupPrompt.text = prompt;
        pickupPanelAnimation = StartCoroutine(
            PanelAnimation(pickupWindowRect, 
            RectTransform.Axis.Horizontal));
    }

    public void HidePickupPanel()
    {
        pickupCanva.SetActive(false);
        StopCoroutine(pickupPanelAnimation);
    }

    IEnumerator PanelAnimation(RectTransform panelTransform, RectTransform.Axis axis)
    {
        if (!rectSize.TryGetValue(panelTransform, out Rect rect))
        {
            rectSize.Add(panelTransform, panelTransform.rect);
            rect = panelTransform.rect;
        }

        Vector2 originalSize = rect.size;
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
