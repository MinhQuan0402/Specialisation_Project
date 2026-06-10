using System.Security.Cryptography;
using UnityEngine;

public abstract class BaseNPC : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    [SerializeField] protected NPCData NPCData;
    [SerializeField] private string interactPrompt = "[E] To Talk";
    [SerializeField] private Vector2 UIOffset = Vector2.zero;

    [Header("Interaction Settings")]
    [SerializeField] protected bool FacePlayerOnInteract = false;
    [SerializeField, Range(-1, 1)] protected int FacingDirection = 1;

    [Header("Flags")]
    [SerializeField, Tooltip("[Optional] flag to set after talking")] private string setFlagOnInteract;

    public string DisplayName => NPCData.NPCName;
    public string InteractPrompt => interactPrompt;

    public virtual bool CanInteract => true;

    protected bool PlayerInRange { get; private set; }

    protected bool hasFlip = false;
    protected bool inInteraction = false;
    protected bool hasInteracted = false;

    public virtual void OnInteract()
    {
        if (inInteraction) return;

        UIManager.Instance.HideInteractionPanel();
        Player player = Player.Instance;
        AlignFacingDirections(player);
        player.Freeze();
        inInteraction = true;
    }

    public virtual void OnInteractionComplete()
    {
        hasInteracted = true;
        inInteraction = false;

        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
        Player.Instance.UnFreeze();

        // Set a world flag if configured
        if (!string.IsNullOrEmpty(setFlagOnInteract))
            GameFlagRegistry.Set(setFlagOnInteract);

        // Fire event — scene controllers and other systems can react
        NPCEventBus.TriggerNPCInteracted(NPCData);

        if (!string.IsNullOrEmpty(setFlagOnInteract))
            GameFlagRegistry.Set(setFlagOnInteract);

        if (hasFlip) transform.localScale = new Vector3(FacingDirection, transform.localScale.y, transform.localScale.z);
        hasFlip = false;
    }

    public void OnPlayerEnterRange()
    {
        PlayerInRange = true;
        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
    }

    public void OnPlayerExitRange()
    {
        PlayerInRange = false;
        UIManager.Instance.HideInteractionPanel();
    }

    private void AlignFacingDirections(Player player)
    {
        float deltaX = player.transform.position.x - transform.position.x;

        if (Mathf.Abs(deltaX) < 0.01f)
            return;

        int directionToPlayer = deltaX > 0 ? 1 : -1;

        // NPC faces player
        if (FacePlayerOnInteract && 
            directionToPlayer != FacingDirection)
        {
            transform.localScale = new Vector3(
                directionToPlayer,
                transform.localScale.y,
                transform.localScale.z);
            hasFlip = true;
        }

        // Player faces NPC
        int directionToNpc = -directionToPlayer;

        if (player.Movement.Comp.FacingDirection != directionToNpc)
        {
            player.Movement.Comp.Flip();
        }
    }
}
