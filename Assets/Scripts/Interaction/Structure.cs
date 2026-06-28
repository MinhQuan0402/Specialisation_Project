using System;
using UnityEngine;

public class Structure : MonoBehaviour, IInteractable
{
    [SerializeField] private string displayName = string.Empty;
    [SerializeField] private string interactPrompt = "[E] To Interact";
    [SerializeField] private Vector2 UIOffset = Vector2.zero;
    [SerializeField] private Bobber bobber;

    protected event Action OnInteractionCompleted;

    public string DisplayName => displayName;

    public string InteractPrompt => interactPrompt;

    public bool CanInteract => true;

    public bool InInteraction { get; protected set; } = false;

    void Start()
    {
        if (bobber != null) bobber.StartBobbing();
    }

    public virtual void OnInteract()
    {
        InInteraction = true;
        UIManager.Instance.HideInteractionPanel();

        OnInteractionCompleted += OnInteractionComplete;
    }

    public virtual void OnPlayerEnterRange()
    {
        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
    }

    public virtual void OnPlayerExitRange()
    {
        UIManager.Instance.HideInteractionPanel();
    }

    public virtual void OnInteractionComplete() 
    {
        OnInteractionCompleted -= OnInteractionComplete;
        InInteraction = false;
        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
    }

    public Vector3 GetPosition() => transform.position;

    protected void TriggerCompletedEvent() => OnInteractionCompleted?.Invoke();
}
