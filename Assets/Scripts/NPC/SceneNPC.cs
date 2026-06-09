using UnityEngine;
using System.Collections.Generic;

public class SceneNPC : BaseNPC
{
    [Header("Identity")]
    [SerializeField] private NPCData NPCData;

    [Header("Dialogue")]
    [SerializeField] private List<DialogueSequence> dialogueSequences;

    [Header("Condition")]
    [SerializeField, Tooltip("[Optional] leave empty = always interactable")] private MonoBehaviour conditionComponent;

    [Header("Flags")]
    [SerializeField, Tooltip("[Optional] flag to set after talking")] private string setFlagOnInteract;
    [SerializeField] private bool repeatableDialogue = false;

    [Header("Item to give")]
    [SerializeField] private WeaponData giftWeapon;
    [SerializeField] private int giftScore;

    private INPCCondition condition;
    private int dialogueIndex = 0;  // which sequence is active
    private bool hasInteracted = false;

    protected virtual void Awake()
    {
        condition = conditionComponent as INPCCondition;
    }

    public override bool CanInteract
    {
        get
        {
            if (!repeatableDialogue && hasInteracted) return false;
            if (condition != null && !condition.IsMet) return false;
            return true;
        }
    }

    public override void OnInteract(Player player)
    {
        base.OnInteract(player);

        switch (NPCData.behaviour)
        {
            case NPCBehaviour.Dialogue:
                HandleDialogue(player);
                break;
            case NPCBehaviour.GiveItem:
                HandleGiveItem(player);
                break;
        }
    }

    // ── Dialogue ─────────────────────────────────────────────
    private void HandleDialogue(Player player)
    {
        if (dialogueSequences == null || dialogueSequences.Count == 0) return;

        var seq = dialogueSequences[
            Mathf.Clamp(dialogueIndex, 0, dialogueSequences.Count - 1)];

        if (!DialogueManager.Instance.IsOpen)
            DialogueManager.Instance.StartSequence(seq, OnDialogueComplete);
        else DialogueManager.Instance.Advance();
    }

    private void OnDialogueComplete()
    {
        hasInteracted = true;

        // Advance to next dialogue sequence (NPC "remembers" past conversations)
        if (dialogueIndex < dialogueSequences.Count - 1)
            dialogueIndex++;

        // Set a world flag if configured
        if (!string.IsNullOrEmpty(setFlagOnInteract))
            GameFlagRegistry.Set(setFlagOnInteract);

        // Fire event — scene controllers and other systems can react
        NPCEventBus.TriggerNPCInteracted(NPCData);
        if (hasFlip) transform.localScale = new Vector3(FacingDirection, transform.localScale.y, transform.localScale.z);
        UIManager.Instance.HideInteractionPanel();
    }


    // ── Give Item ────────────────────────────────────────────
    private void HandleGiveItem(Player player)
    {
        if (giftWeapon != null)
            player.InventorySystem.TryToAddWeapon(giftWeapon);

        if (giftScore > 0) GameManager.Instance.AddScore(giftScore);

        hasInteracted = true;
        if (!string.IsNullOrEmpty(setFlagOnInteract))
            GameFlagRegistry.Set(setFlagOnInteract);

        NPCEventBus.TriggerNPCInteracted(NPCData);
    }

    // ── Public API ────────────────────────────────────────────
    public void SetDialogueIndex(int index)
    {
        dialogueIndex = Mathf.Clamp(index, 0, dialogueSequences.Count - 1);
    }

    public void ResetNPC()
    {
        hasInteracted = false;
        dialogueIndex = 0;
    }
}