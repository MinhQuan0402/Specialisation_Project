using System;
using UnityEngine;

public class InteractableDetector : CoreComponent
{
    public Action<IInteractable> OnTryInteract;

    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask interactionLayer;

    private IInteractable currInteraction;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        OnTryInteract += HandleNonItemInteraction;
    }

    private void OnDisable()
    {
        OnTryInteract -= HandleNonItemInteraction;
    }

    private void Update()
    {
        DetectNearestIntertion();
    }

    [ContextMenu("TryInteract")]
    public void TryInteract(bool inputValue)
    {
        if (!inputValue || currInteraction is null)
            return;

        OnTryInteract?.Invoke(currInteraction);
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
            currInteraction?.OnPlayerEnterRange();
        }
    }

    void HandleNonItemInteraction(IInteractable currInteraction)
    {
        if (currInteraction is Item pickup)
            return;

        currInteraction.OnInteract();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
