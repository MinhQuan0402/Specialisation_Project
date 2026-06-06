using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public ItemInstance item;

    [SerializeField] private Transform spriteTransform;

    [SerializeField] private float floatingSpeed = 0.1f;
    [SerializeField] private float floatingHeight = 0.5f;

    [SerializeField] private Vector2 offset;

    private Vector2 startPos;

    public string DisplayName => item.itemData.name;

    public virtual string InteractPrompt => "Press E to pickup";

    public bool CanInteract => true;

    void Start()
    {
        spriteTransform.GetComponent<SpriteRenderer>().sprite = item.itemData.itemImage;
        startPos = spriteTransform.position;
    }

    void Update()
    {
        FloatingEffect();
    }

    public ItemInstance TakeItem()
    {
        Destroy(gameObject);
        return item;
    }

    private void FloatingEffect()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatingSpeed) * floatingHeight;
        spriteTransform.position = new Vector3(spriteTransform.position.x, newY);
    }

    public virtual void OnPlayerEnterRange(Player player)
    {
        UIManager.Instance.SetPickupPanel(startPos + offset, InteractPrompt);
    }

    public void OnPlayerExitRange()
    {
        UIManager.Instance.HidePickupPanel();
    }

    public virtual void OnInteract(Player player)
    {
        player.InventorySystem.TryToAddItem(TakeItem());
        UIManager.Instance.HidePickupPanel();
    }
}
