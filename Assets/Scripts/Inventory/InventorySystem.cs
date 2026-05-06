using UnityEngine;
using UnityEngine.InputSystem;
public class InventorySystem : MonoBehaviour
{
    public Inventory inventory;
    [SerializeField] private UIInventoryPage inventoryUI;

    private bool interacted = false;

    public void OnInteractionInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            interacted = true;
        }
        else
        {
            interacted = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out Item item))
        {
            if (interacted)
            {
                inventory.AddItem(item.TakeItem());
                interacted = false;
            }
        }
    }
}
