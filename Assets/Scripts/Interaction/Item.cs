using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour, IInteractable
{
    public ItemInstance item;

    [SerializeField] private SpriteRenderer itemIcon;

    [SerializeField] private Vector2 UIOffset;

    public UnityEvent<ItemInstance, bool> OnInteractEvent;

    public string DisplayName => item.itemData.name;

    public virtual string InteractPrompt => "[E] To Pickup";

    public bool CanInteract => true;

    void Start()
    {
        if (itemIcon != null && item.itemData != null)
            itemIcon.sprite = item.itemData.itemImage;
    }

    public ItemInstance TakeItem()
    {
        Destroy(gameObject);
        return item;
    }

    public virtual void OnPlayerEnterRange()
    {
        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
    }

    public void OnPlayerExitRange()
    {
        UIManager.Instance.HideInteractionPanel();
    }

    public virtual void OnInteract()
    {
        bool result = Player.Instance.InventorySystem.TryToAddItem(TakeItem());
        UIManager.Instance.HideInteractionPanel();
        OnInteractEvent.Invoke(item, result);
    }

    public virtual void OnInteractionComplete() { }
}
