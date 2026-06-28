using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class WeaponSwapUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private WeaponInfoUI newWeaponInfo;
    [SerializeField] private WeaponInfoUI[] weaponSwapUIChoices;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 1.0f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private AnimationCurve fadeCurve;

    private InventorySystem inventorySystem;
    private Action<WeaponSwapChoice> choiceSelectedCallback;

    private WeaponSwapChoice currentSelectedChoice;
    private CanvasGroup canvasGroup;

    private void HandleChoiceRequested(WeaponSwapChoiceRequest choiceRequest)
    {
        GameManager.Instance.ChangeState(GameState.Paused);
        StartCoroutine(FadeAnimation());

        currentSelectedChoice  = choiceRequest.Choices[0];
        choiceSelectedCallback = choiceRequest.Callback;

        newWeaponInfo.PopulateUI(choiceRequest.NewWeaponData);

        foreach (var weaponSwapChoiceUi in weaponSwapUIChoices)
        {
            weaponSwapChoiceUi.ChoiceToggle.isOn = true;
            weaponSwapChoiceUi.TakeRelevantChoice(choiceRequest.Choices);
        }

        weaponSwapUIChoices.Where(choice => choice.CombatInputs == CombatInputs.primary).First().ChoiceToggle.isOn = true;
    }

    private void HandleChoiceSelected(WeaponSwapChoice newWeaponSwapChoice)
    {
        if (currentSelectedChoice == newWeaponSwapChoice) return;

        currentSelectedChoice = newWeaponSwapChoice;
    }

    public void OnSwap()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
        choiceSelectedCallback?.Invoke(currentSelectedChoice);
        StartCoroutine(FadeAnimation(true));
    }

    public void OnCancel()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
        StartCoroutine(FadeAnimation(true));
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventorySystem = Player.Instance.Core.GetCoreComponent<InventorySystem>();
    }

    private void OnEnable()
    {
        inventorySystem.OnChoiceRequested += HandleChoiceRequested;
        foreach (var weaponSwapChoiceUI in weaponSwapUIChoices)
        {
            weaponSwapChoiceUI.OnChoiceActive += HandleChoiceSelected;
        }
    }

    private void OnDisable()
    {
        inventorySystem.OnChoiceRequested -= HandleChoiceRequested;

        foreach (var weaponSwapChoiceUI in weaponSwapUIChoices)
        {
            weaponSwapChoiceUI.OnChoiceActive += HandleChoiceSelected;
        }
    }

    IEnumerator FadeAnimation(bool reverse = false)
    {
        float progress = 0.0f;
        while (progress < fadeDuration)
        {
            progress += Time.unscaledDeltaTime * fadeSpeed;
            float currentAlpha = reverse ? Mathf.Lerp(1.0f, 0.0f, fadeCurve.Evaluate(progress / fadeDuration)) :
                                           Mathf.Lerp(0.0f, 1.0f, fadeCurve.Evaluate(progress / fadeDuration));
            canvasGroup.alpha = currentAlpha;
            yield return null;
        }

        if (reverse)
        {
            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            GameManager.Instance.ChangeState(GameState.Playing);
        }
        else
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        yield return null;
    }
}
