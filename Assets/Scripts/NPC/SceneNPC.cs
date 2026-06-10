using UnityEngine;
using System.Collections.Generic;

public class SceneNPC : BaseNPC
{
    [Header("Dialogue")]
    [SerializeField] private List<DialogueSequence> dialogueSequences;
    [SerializeField] private bool repeatableDialogue = false;

    [Header("Condition")]
    [SerializeField, Tooltip("[Optional] leave empty = always interactable")] private MonoBehaviour conditionComponent;

    [Header("Item to give")]
    [SerializeField] private WeaponData giftWeapon;
    [SerializeField] private int giftScore;

    private INPCCondition condition;
    private int dialogueIndex = 0;  // which sequence is active
    

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

    public override void OnInteract()
    {
        base.OnInteract();

        switch (NPCData.behaviour)
        {
            case NPCBehaviour.Dialogue:
                HandleDialogue();
                break;
            case NPCBehaviour.GiveItem:
                HandleGiveItem();
                break;
        }
    }

    public override void OnInteractionComplete()
    {
        base.OnInteractionComplete();

        hasInteracted = true;

        if (dialogueIndex < dialogueSequences.Count - 1)
            dialogueIndex++;

        switch (NPCData.behaviour)
        {
            case NPCBehaviour.Dialogue:
                break;
            case NPCBehaviour.GiveItem:
                break;
        }
    }

    // ── Dialogue ─────────────────────────────────────────────
    private void HandleDialogue()
    {
        if (dialogueSequences == null || dialogueSequences.Count == 0) return;

        var seq = dialogueSequences[
            Mathf.Clamp(dialogueIndex, 0, dialogueSequences.Count - 1)];

        if (!DialogueManager.Instance.IsOpen)
            DialogueManager.Instance.StartSequence(seq, OnInteractionComplete);
        else DialogueManager.Instance.Advance();
    }

    // ── Give Item ────────────────────────────────────────────
    private void HandleGiveItem()
    {
        if (giftWeapon != null)
            Player.Instance.InventorySystem.TryToAddWeapon(giftWeapon);

        if (giftScore > 0) GameManager.Instance.AddScore(giftScore);

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