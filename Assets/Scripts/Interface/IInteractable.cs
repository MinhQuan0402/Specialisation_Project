using UnityEngine;

public interface IInteractable
{
    string DisplayName { get; }
    string InteractPrompt { get; }   // e.g. "[E] Talk", "[E] Shop"
    bool CanInteract { get; }      // e.g. false if shop is sold out
    void OnPlayerEnterRange();
    void OnPlayerExitRange();
    void OnInteract();
    void OnInteractionComplete();
    Vector3 GetPosition();
}