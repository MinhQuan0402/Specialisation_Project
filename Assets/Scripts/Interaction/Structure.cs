using UnityEngine;

public class Structure : MonoBehaviour, IInteractable
{
    [SerializeField] private string displayName = string.Empty;
    [SerializeField] private string interactPrompt = "[E] To Interact";
    [SerializeField] private Vector2 UIOffset = Vector2.zero;
    [SerializeField] private Bobber bobber;

    public string DisplayName => displayName;

    public string InteractPrompt => interactPrompt;

    public bool CanInteract => true;

    void Start()
    {
        if (bobber != null) bobber.StartBobbing();
    }

    public void OnInteract()
    {
        UIManager.Instance.HideInteractionPanel();
    }

    public void OnPlayerEnterRange()
    {
        UIManager.Instance.EnableInteractionPanel(transform.position + (Vector3)UIOffset, InteractPrompt);
    }

    public void OnPlayerExitRange()
    {
        UIManager.Instance.HideInteractionPanel();
    }

    public void OnInteractionComplete() { }
}
