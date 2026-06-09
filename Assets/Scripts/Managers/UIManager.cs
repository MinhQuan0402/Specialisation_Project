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

    [Header("Interaction Elements")]
    [SerializeField] private GameObject    pickupCanva;
    [SerializeField] private RectTransform pickupWindowRect;
    [SerializeField] private Text pickupPrompt;
    
    [Header("Dialogue Elements")]
    [SerializeField] private RectTransform dialoguePromptPanel;
    [SerializeField] private Image dialogueNPCIcon;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueNPCName;
    [SerializeField] private TMPro.TextMeshProUGUI dialoguePromptText;

    [Header("Death Feedback Elements")]
    [SerializeField] private RectTransform deathFeedbackPanel;
    [SerializeField] private TMPro.TextMeshProUGUI dFbkTimer;
    [SerializeField] private Text hintText;

    [Header("Transition")]
    [SerializeField] private CanvasGroup fadeOverlay;   // full-screen black fade
    [SerializeField] private float panelAnimationSpeed = 1.0f;
    [SerializeField] private float panelAnimDuration = 1.0f;

    private readonly Dictionary<RectTransform, Coroutine> coroutines = new();
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

    public void EnableInteractionPanel(Vector2 position, string prompt)
    {
        pickupCanva.SetActive(true);
        pickupCanva.transform.position = position;
        pickupPrompt.text = prompt;

        if (coroutines.ContainsKey(pickupWindowRect))
        {
            coroutines[pickupWindowRect] = StartCoroutine(
                            PanelAnimation(pickupWindowRect,
                            RectTransform.Axis.Horizontal));
            return;
        }

        coroutines.Add(pickupWindowRect, StartCoroutine(
                        PanelAnimation(pickupWindowRect,
                        RectTransform.Axis.Horizontal)));
    }

    public void HideInteractionPanel()
    {
        pickupCanva.SetActive(false);
        StopCoroutine(coroutines[pickupWindowRect]);
    }

    public void EnableDialoguePrompt(Sprite icon, string npcName, string prompt)
    {
        dialoguePromptPanel.gameObject.SetActive(true);
        dialogueNPCIcon.sprite = icon;
        dialogueNPCName.text = npcName;
        dialoguePromptText.text = prompt;

        if (coroutines.ContainsKey(dialoguePromptPanel))
        {
            coroutines[dialoguePromptPanel] = StartCoroutine(
                            PanelAnimation(dialoguePromptPanel,
                            RectTransform.Axis.Horizontal));
            return;
        }

        coroutines.Add(dialoguePromptPanel, StartCoroutine(
                        PanelAnimation(dialoguePromptPanel,
                        RectTransform.Axis.Horizontal)));
    }

    public void UpdateDialoguePrompt(Sprite icon, string npcName, string prompt)
    {
        dialogueNPCIcon.sprite = icon;
        dialogueNPCName.text = npcName;
        dialoguePromptText.text = prompt;
    }

    public TMPro.TextMeshProUGUI DialogueText => dialoguePromptText;

    public void HideDialoguePrompt()
    {
        dialoguePromptPanel.gameObject.SetActive(false);
        StopCoroutine(coroutines[dialoguePromptPanel]);
    }

    public void EnableFeedbackPrompt(string timer, string prompt)
    {
        deathFeedbackPanel.gameObject.SetActive(true);
        hintText.text = prompt;
        dFbkTimer.text = timer;

        if (coroutines.ContainsKey(deathFeedbackPanel))
        {
            coroutines[deathFeedbackPanel] = StartCoroutine(
                            PanelAnimation(deathFeedbackPanel,
                            RectTransform.Axis.Vertical));
            return;
        }

        coroutines.Add(deathFeedbackPanel, StartCoroutine(
                        PanelAnimation(deathFeedbackPanel,
                        RectTransform.Axis.Vertical)));
    }

    public Text HintText => hintText;

    public void HideFeedbackPrompt()
    {
        StopCoroutine(coroutines[deathFeedbackPanel]);
        coroutines[deathFeedbackPanel] = StartCoroutine(
                    PanelAnimationReverse(deathFeedbackPanel, 
                    RectTransform.Axis.Vertical));
    }

    public void SetHUDActive(bool enable) => hudPanel.SetActive(enable);

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

    IEnumerator PanelAnimationReverse(RectTransform panelTransform, RectTransform.Axis axis)
    {
        Vector2 originalSize = panelTransform.rect.size;
        float progress = 0f;
        while (progress < panelAnimDuration)
        {
            progress += Time.deltaTime * panelAnimationSpeed;
            float currentSize = Mathf.Lerp(axis == RectTransform.Axis.Horizontal ?
                                        originalSize.x : originalSize.y, 0.0f,
                                        progress / panelAnimDuration);
            panelTransform.SetSizeWithCurrentAnchors(axis, currentSize);
            yield return null;
        }

        panelTransform.gameObject.SetActive(false);
        yield return null;
    }
}