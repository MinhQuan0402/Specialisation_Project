// NPCEventBus — for anything that wants to react to NPC interactions
using System;

public static class NPCEventBus
{
    public static event Action<DialogueNPC> OnDialogueComplete;
    //public static event Action<ShopNPC, ShopItem> OnItemPurchased;
    public static event Action<TutorialStep> OnTutorialStepNPCComplete;

    public static void TriggerDialogueComplete(DialogueNPC npc) => OnDialogueComplete?.Invoke(npc);
    //public static void TriggerItemPurchased(ShopNPC npc, ShopItem item) => OnItemPurchased?.Invoke(npc, item);
    public static void TriggerTutorialStepNPCComplete(TutorialStep s) => OnTutorialStepNPCComplete?.Invoke(s);
}