using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour, IInteractable
{
    public ItemInstance item;

    [SerializeField] private SpriteRenderer itemIcon;
    [SerializeField] private Bobber bobber;

    [SerializeField] private Vector2 UIOffset;

    public UnityEvent<ItemInstance, bool> OnInteractEvent;

    public string DisplayName => item.itemData.name;

    public virtual string InteractPrompt => "[E] To Pickup";

    public bool CanInteract => true;

    void Start()
    {
        itemIcon.sprite = item.itemData.itemImage;
        bobber.StartBobbing();
    }

    public ItemInstance TakeItem()
    {
        Destroy(gameObject);
        return item;
    }

    public virtual void OnPlayerEnterRange(Player player)
    {
        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
    }

    public void OnPlayerExitRange()
    {
        UIManager.Instance.HideInteractionPanel();
    }

    public virtual void OnInteract(Player player)
    {
        bool result = player.InventorySystem.TryToAddItem(TakeItem());
        UIManager.Instance.HideInteractionPanel();
        OnInteractEvent.Invoke(item, result);
    }
}
