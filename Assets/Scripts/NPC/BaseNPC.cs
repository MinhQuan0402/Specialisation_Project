using System.Security.Cryptography;
using UnityEngine;

public abstract class BaseNPC : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    [SerializeField] protected CharacterData NPCData;
    [SerializeField] private string interactPrompt = "[E] To Talk";
    [SerializeField] private Vector2 UIOffset = Vector2.zero;

    [Header("Interaction Settings")]
    [SerializeField] protected bool FacePlayerOnInteract = false;

    [Header("Flags")]
    [SerializeField, Tooltip("[Optional] flag to set after talking")] private string setFlagOnInteract;

    protected Core Core { get; private set; }
    protected CoreComp<Movement> Movement { get; private set; }

    public string DisplayName => NPCData.CharacterName;
    public string InteractPrompt => interactPrompt;

    public virtual bool CanInteract => true;

    protected bool PlayerInRange { get; private set; }

    protected bool hasFlip = false;
    protected bool inInteraction = false;
    protected bool hasInteracted = false;
    

    private void Start()
    {
        Core = GetComponentInChildren<Core>();
        Movement = new CoreComp<Movement>(Core);
    }

    public virtual void OnInteract()
    {
        UIManager.Instance.HideInteractionPanel();
        Player player = Player.Instance;
        AlignFacingDirections(player);
        player.Paused();
        inInteraction = true;
    }

    public virtual void OnInteractionComplete()
    {
        hasInteracted = true;
        inInteraction = false;

        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
        Player.Instance.Unpaused();

        // Set a world flag if configured
        if (!string.IsNullOrEmpty(setFlagOnInteract))
            GameFlagRegistry.Set(setFlagOnInteract);

        // Fire event — scene controllers and other systems can react
        NPCEventBus.TriggerNPCInteracted(NPCData);

        if (!string.IsNullOrEmpty(setFlagOnInteract))
            GameFlagRegistry.Set(setFlagOnInteract);

        if (hasFlip) Movement.Comp.Flip();
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
            directionToPlayer != -Movement.Comp.FacingDirection)
        {
            Movement.Comp.Flip();
            hasFlip = true;
        }

        // Player faces NPC
        int directionToNpc = -directionToPlayer;

        if (player.Movement.Comp.FacingDirection != directionToNpc)
        {
            player.Movement.Comp.Flip();
        }
    }

    public Vector3 GetPosition() => transform.position;
}
