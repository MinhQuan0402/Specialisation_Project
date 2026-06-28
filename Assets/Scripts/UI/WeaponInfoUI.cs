using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private Image weaponIcon;

    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private PageFlipperUI weaponDescription;
    [SerializeField] private RectTransform statsContent;
    [SerializeField] private Toggle infoToggle;
 
    [Header("Weapon Choice")]
    [SerializeField] private CombatInputs weaponInputs;
    [SerializeField] private Toggle choiceToggle;

    public CombatInputs CombatInputs => weaponInputs;
    public Toggle ChoiceToggle => choiceToggle;

    public Action<WeaponSwapChoice> OnChoiceActive;

    private WeaponData weaponData;
    private WeaponSwapChoice weaponSwapChoice;

    private void ResetUI()
    {
        infoToggle.isOn = true;
    }

    public void PopulateUI(WeaponData data)
    {
        if (data == null)
            return;

        ResetUI();
        weaponData = data;

        weaponIcon.sprite = weaponData.itemImage;
        weaponName.SetText(weaponData.itemName);
        weaponDescription.SetText(weaponData.itemDescription);

        for(int i = 0; i < statsContent.childCount; ++i) Destroy(statsContent.GetChild(i).gameObject);
        foreach(var component in weaponData.ComponentData)
        {
            component.PopulateStatUI(statsContent);
        }

        UIManager.Instance.CreateWeaponStatUI(statsContent,
            new TextUIInput("Attack Speed: ", Color.black),
            new TextUIInput(weaponData.AttackSpeed.ToString() + "/s", Color.black));

        UIManager.Instance.CreateWeaponStatUI(statsContent,
            new TextUIInput("Stamina Used: ", Color.black),
            new TextUIInput(weaponData.WeaponStamina.ToString(), Color.darkBlue));

        UIManager.Instance.CreateWeaponStatUI(statsContent,
            new TextUIInput(weaponData.PassiveName, Color.black),
            new TextUIInput(weaponData.PassiveDesc.ToString(), Color.white), false);


        if (choiceToggle != null)
        {
            choiceToggle.onValueChanged.RemoveAllListeners();

            choiceToggle.onValueChanged.AddListener(HandleToggleChoiceActive);
        }
    }

    void HandleToggleChoiceActive(bool isActive)
    {
        if (!isActive) return;
        OnChoiceActive?.Invoke(weaponSwapChoice);
    }

    public void TakeRelevantChoice(WeaponSwapChoice[] choices)
    {
        var inputIndex = (int)weaponInputs;

        if (choices.Length <= inputIndex)
        {
            return;
        }

        SetChoice(choices[inputIndex]);
    }

    private void SetChoice(WeaponSwapChoice choice)
    {
        weaponSwapChoice = choice;

        PopulateUI(choice.WeaponData);
    }
}
