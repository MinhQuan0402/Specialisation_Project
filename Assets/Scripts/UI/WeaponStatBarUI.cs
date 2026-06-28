using TMPro;
using UnityEngine;

public class WeaponStatBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI stat;

    public void SetUIText(TextUIInput label, TextUIInput stat)
    {
        if (this.label != null)
        {
            this.label.text  = label.text;
            this.label.color = label.textColor;
        }

        if (this.stat  != null)
        {
            this.stat.text  = stat.text;
            this.stat.color = stat.textColor;
        }
    }
}
