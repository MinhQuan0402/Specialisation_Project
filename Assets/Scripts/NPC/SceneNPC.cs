using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class SceneNPC : BaseNPC
{
    [Header("Dialogue")]
    [SerializeField] private List<DialogueSequence> dialogueSequences;
    [SerializeField] private bool repeatableDialogue = false;

    [Header("Condition")]
    [SerializeField, Tooltip("[Optional] leave empty = always interactable")] private MonoBehaviour conditionComponent;

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
        if (inInteraction) return;

        base.OnInteract();

        switch (NPCData.behaviour)
        {
            case NPCBehaviour.Dialogue:
                HandleDialogue();
                break;
        }
    }

    public override void OnInteractionComplete()
    {
        base.OnInteractionComplete();

        hasInteracted = true;

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

        DialogueManager.Instance.StartSequence(seq, OnInteractionComplete);
    }

    // ── Public API ────────────────────────────────────────────
    public void SetDialogueIndex(int index)
    {
        hasInteracted |= (index >= 0);
        dialogueIndex = Mathf.Clamp(index, 0, dialogueSequences.Count - 1);
    }

    public void ResetNPC()
    {
        hasInteracted = false;
        dialogueIndex = 0;
    }
}