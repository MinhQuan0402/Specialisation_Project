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

public class TextUIInput
{
    public string text;
    public Color textColor;

    public TextUIInput(string text, Color textColor)
    {
        this.text = text;
        this.textColor = textColor;
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
    [SerializeField] private RectTransform         keysPanel;
    [SerializeField] private TMPro.TextMeshProUGUI keysInfo; 

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
    [SerializeField] private GameObject pickupCanva;
    [SerializeField] private RectTransform pickupWindowRect;
    [SerializeField] private Text pickupPrompt;

    [Header("Dialogue Elements")]
    [SerializeField] private RectTransform dialoguePromptPanel;
    [SerializeField] private Image dialogueNPCIcon;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueNPCName;
    [SerializeField] private TMPro.TextMeshProUGUI dialoguePromptText;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueInstructionText;

    [Header("Death Feedback Elements")]
    [SerializeField] private RectTransform deathFeedbackPanel;
    [SerializeField] private TMPro.TextMeshProUGUI dFbkTimer;
    [SerializeField] private Text hintText;

    [Header("Weapon Swap Elements")]
    [SerializeField] private WeaponStatBarUI weaponStatBarUIPrefab;
    [SerializeField] private WeaponStatBarUI specialStatBarUIPrefab;

    [Header("Transition")]
    [SerializeField] private CinematicBars cinematicBars;
    [SerializeField] private float panelAnimationSpeed = 1.0f;
    [SerializeField] private float panelAnimDuration = 1.0f;
    [SerializeField] private float pulseSpeed = 2.0f;
    [SerializeField] private AnimationCurve pulseSizeCurve;

    private readonly Dictionary<RectTransform, Coroutine> coroutines = new();
    private readonly Dictionary<RectTransform, Rect> rectSize = new();

    public InputSystem_Actions InputActions { get; private set; }

    private readonly Dictionary<TMPro.TextMeshProUGUI, Coroutine> textPulseAnimation = new();

    protected override void Awake()
    {
        base.Awake();

        InputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        InputActions?.Enable();
    }

    private void OnDisable()
    {
        InputActions?.Disable();
    }

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
        if (index >= weaponSlots.Length || index < 0) return;
        if (itemData == null) return;

        weaponSlots[index].Set(itemData.itemName, itemData.itemImage);
    }

    public void UpdateKeysUI(int totalKeys)
    {
        if (!keysPanel.gameObject.activeSelf) keysPanel.gameObject.SetActive(true);
        keysInfo.text = "x" + totalKeys.ToString();
        coroutines[keysPanel] = StartCoroutine(PulseSize(keysPanel));
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

    public void UpdateInteractionPanelPos(Vector2 position)
    {
        pickupCanva.transform.position = position;
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

    public void EnableSkipDialogueInstruction()
    {
        dialogueInstructionText.text = "Press E to skip";
        textPulseAnimation[dialogueInstructionText] = StartCoroutine(PulseText(dialogueInstructionText));
    }

    public void EnableContinueDialogueInstruction()
    {
        dialogueInstructionText.text = "Press E to continue";
        textPulseAnimation[dialogueInstructionText] = StartCoroutine(PulseText(dialogueInstructionText));
    }

    public TMPro.TextMeshProUGUI DialogueText => dialoguePromptText;

    public void HideDialoguePrompt()
    {
        dialogueInstructionText.text = "";
        dialogueInstructionText.color = new Color(0, 0, 0, 0);
        dialoguePromptPanel.gameObject.SetActive(false);
        StopCoroutine(coroutines[dialoguePromptPanel]);
        StopCoroutine(textPulseAnimation[dialogueInstructionText]);
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

    public void CreateWeaponStatUI(RectTransform statsContent, 
                                   TextUIInput label, 
                                   TextUIInput desc, bool normalStat = true)
    {
        WeaponStatBarUI weaponStat = normalStat ? Instantiate(weaponStatBarUIPrefab, statsContent) : 
                                                  Instantiate(specialStatBarUIPrefab, statsContent);
        weaponStat.SetUIText(label, desc);
    }

    public void ActivateCinematicBar(float time)
    {
        if (cinematicBars == null) return;
        cinematicBars.Show(time);
    }

    public void DeactivateCinematicBar(float time)
    {
        if (cinematicBars == null) return;
        cinematicBars.Hide(time);
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
            progress += Time.unscaledDeltaTime * panelAnimationSpeed;
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
            progress += Time.unscaledDeltaTime * panelAnimationSpeed;
            float currentSize = Mathf.Lerp(axis == RectTransform.Axis.Horizontal ?
                                        originalSize.x : originalSize.y, 0.0f,
                                        progress / panelAnimDuration);
            panelTransform.SetSizeWithCurrentAnchors(axis, currentSize);
            yield return null;
        }

        panelTransform.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator PanelFadeAnimation(CanvasGroup canvasGroup, bool reverse = false)
    {
        float progress = 0.0f;
        while (progress < panelAnimDuration)
        {
            progress += Time.unscaledDeltaTime * panelAnimationSpeed;
            float currentAlpha = reverse ? Mathf.Lerp(1.0f, 0.0f, progress / panelAnimDuration) :
                                           Mathf.Lerp(0.0f, 1.0f, progress / panelAnimDuration);
            canvasGroup.alpha = currentAlpha;
            yield return null;
        }

        yield return null;
    }

    IEnumerator PulseText(TMPro.TextMeshProUGUI text)
    {
        Color textColor = text.color;
        while(true)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed);
            textColor.a = pulse;
            text.color  = textColor;
            yield return null;
        }
    }

    IEnumerator PulseSize(RectTransform panelTransform)
    {
        float startTime = Time.time;
        float currSize = 0;
        while(currSize < 1)
        {
            currSize = Mathf.Lerp(0.0f, 1.0f, pulseSizeCurve.Evaluate((Time.time - startTime) * panelAnimationSpeed));
            panelTransform.localScale = new Vector3(currSize, currSize, 1.0f);
            yield return null;
        }

        yield return null;
    }
}