using UnityEngine;

public abstract class BaseNPC : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    [SerializeField] private string displayName;
    [SerializeField] private string interactPrompt = "[E] To Talk";
    [SerializeField] private Vector2 UIOffset = Vector2.zero;

    [SerializeField] protected bool FacePlayerOnInteract = false;
    [SerializeField, Range(-1, 1)] protected int FacingDirection = 1;

    protected bool hasFlip = false;

    public string DisplayName => displayName;
    public string InteractPrompt => interactPrompt;

    public virtual bool CanInteract => true;

    protected bool PlayerInRange { get; private set; }

    protected bool interactOnce = false;

    public virtual void OnInteract(Player player)
    {
        if (!interactOnce)
        {
            Vector2 dir = new(player.transform.position.x - transform.position.x, 0.0f);
            if (Mathf.Sign(dir.normalized.x) == player.Movement.Comp.FacingDirection)
                player.Movement.Comp.Flip();
            if (FacePlayerOnInteract && Mathf.Sign(dir.normalized.x) != FacingDirection)
            {
                transform.Rotate(0.0f, 180.0f, 0.0f);
                hasFlip = true;
            }
            player.Freeze();
            interactOnce = true;
        }
    }

    public void OnPlayerEnterRange(Player player)
    {
        PlayerInRange = true;
        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
    }

    public void OnPlayerExitRange()
    {
        PlayerInRange = false;
        UIManager.Instance.HideInteractionPanel();
    }
}
