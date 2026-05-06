using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryDescription : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private TMP_Text itemNameText;

    [SerializeField]
    private TMP_Text itemDescriptionText;

    private void Awake()
    {
        ResetDescription();
    }

    public void ResetDescription()
    {
        this.itemImage.gameObject.SetActive(false);
        this.itemNameText.text = string.Empty;
        this.itemDescriptionText.text = string.Empty;
    }

    public void SetDescription(Sprite itemSprite, string itemName, string itemDescription)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = itemSprite;
        this.itemNameText.text = itemName;
        this.itemDescriptionText.text = itemDescription;
    }
}
