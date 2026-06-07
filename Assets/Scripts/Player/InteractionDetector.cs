using UnityEngine;

public class InteractionDetector : CoreComponent
{
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask interactionLayer;

    private IInteractable currInteraction;
    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = core.Root.GetComponent<Player>();
    }

    private void Update()
    {
        DetectNearestIntertion();

        if (currInteraction != null && currInteraction.CanInteract &&
            player.InputHandler.InteractionInput)
        {
            player.InputHandler.UseInteractionInput();
            currInteraction.OnInteract(player);
        }
    }

    private void DetectNearestIntertion()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, detectionRadius, interactionLayer);

        IInteractable nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent<IInteractable>(out var interactable)) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < nearestDist) { nearest = interactable; nearestDist = dist; }
        }

        // Changed NPC — notify old and new
        if (nearest != currInteraction)
        {
            currInteraction?.OnPlayerExitRange();
            currInteraction = nearest;
            currInteraction?.OnPlayerEnterRange(player);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
