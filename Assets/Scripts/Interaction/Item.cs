using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemInstance item;
    public ItemInstance GetItemContext => item;

    [SerializeField] private SpriteRenderer itemIcon;

    [SerializeField] private Vector2 UIOffset;

    [SerializeField] private Collider2D itemCollider;
    [SerializeField] private float magnetForce = 30.0f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float itemRadius = 2.0f;

    public UnityEvent<ItemInstance> OnInteractEvent;

    public string DisplayName => item.itemData.name;

    public virtual string InteractPrompt => "[E] To Pickup";

    public bool CanInteract => true;

    private bool playerInRange = false;

    private bool isInteracted = false;

    public Rigidbody2D Rigidbody2D
    {
        get
        {
            if (rigidbody2DCache == null) rigidbody2DCache = GetComponent<Rigidbody2D>();
            return rigidbody2DCache;
        }
    }

    private Rigidbody2D rigidbody2DCache;

    protected virtual void Start()
    {
        name = $"ItemPickup ({item.itemData.itemName})";
        rigidbody2DCache = GetComponent<Rigidbody2D>();
        if (itemIcon != null && item.itemData != null)
            itemIcon.sprite = item.itemData.itemImage;
    }

    protected virtual void Update()
    {
        if (!isInteracted) return;

        Vector2 directionToPlayer = Player.Instance.transform.position - transform.position;
        Rigidbody2D.AddForce(directionToPlayer * magnetForce);

        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, itemRadius, playerMask);
        if (playerCollider != null) Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (!playerInRange) return;

        UIManager.Instance.UpdateInteractionPanelPos(transform.position + (Vector3)UIOffset);
    }

    public void SetItemInstance(ItemInstance item)
    {
        this.item = item;
    }

    public virtual void OnPlayerEnterRange()
    {
        if (isInteracted) return;

        playerInRange = true;
        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
    }

    public void OnPlayerExitRange()
    {
        if (isInteracted) return;

        playerInRange = false;
        UIManager.Instance.HideInteractionPanel();
    }

    public virtual void OnInteract()
    {
        if (isInteracted) return;

        if (itemCollider) itemCollider.enabled = false;
        isInteracted = true;
        UIManager.Instance.HideInteractionPanel();
        OnInteractEvent.Invoke(item);
    }

    public virtual void OnInteractionComplete() { }

    public Vector3 GetPosition() => transform.position;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemRadius);

        if (isInteracted)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Player.Instance.transform.position);
        }
    }
}
