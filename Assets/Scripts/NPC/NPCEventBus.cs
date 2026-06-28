// NPCEventBus — for anything that wants to react to NPC interactions
using System;

public static class NPCEventBus
{
    public static event Action<CharacterData > OnNPCInteracted;
    public static event Action<SceneNPC> OnDialogueComplete;
    //public static event Action<ShopNPC, ShopItem> OnItemPurchased;

    public static void TriggerNPCInteracted(CharacterData data) => OnNPCInteracted?.Invoke(data);
    public static void TriggerDialogueComplete(SceneNPC npc) => OnDialogueComplete?.Invoke(npc);
    //public static void TriggerItemPurchased(ShopNPC npc, ShopItem item) => OnItemPurchased?.Invoke(npc, item);
}