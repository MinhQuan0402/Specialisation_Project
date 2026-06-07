using UnityEngine;

public class DialogueNPC : BaseNPC
{
    [SerializeField] private DialogueLine[] lines;
    [SerializeField] private bool repeatable = false;

    private int currentLine = 0;
    private bool hasSpoken = false;

    public override bool CanInteract => repeatable || !hasSpoken || currentLine > 0;

    public override void OnInteract(Player player)
    {
        base.OnInteract(player);

        if (currentLine >= lines.Length)
        {
            // Dialogue finished
            currentLine = 0;
            hasSpoken = true;
            interactOnce = false;
            DialogueManager.Instance.EndDialogue();
            player.UnFreeze();
            if (hasFlip)
            {
                transform.Rotate(0.0f, -180.0f, 0.0f);
                hasFlip = false;
            }
            // Fire event other systems can hear
            NPCEventBus.TriggerDialogueComplete(this);
            return;
        }

        DialogueManager.Instance.ShowLine(lines[currentLine]);
        currentLine++;
    }
}