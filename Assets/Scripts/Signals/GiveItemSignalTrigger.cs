using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class GiveItemEntry
{
    public SignalAsset signal;
    public DiscardedItemSpawner itemDiscardSpawner;
    public ItemInstance itemToGive;
    public int scoreToGive;
}

public class GiveItemSignalTrigger : MonoBehaviour
{
    [SerializeField] private List<GiveItemEntry> entries;

    public void GiveItem (DiscardedItemSpawner DiscardedItemSpawner)
    {
        var entry = entries.Where(e => e.itemDiscardSpawner == DiscardedItemSpawner).First();
        if (entry != null)
        {
            if (entry.itemToGive != null)
                entry.itemDiscardSpawner.HandleItemDiscarded(entry.itemToGive);

            if (entry.scoreToGive > 0)
                GameManager.Instance.AddScore(entry.scoreToGive);
        }
    }

    public void OnNotify(SignalAsset signal)
    {
        foreach (var entry in entries)
        {
            if (entry.signal == null) continue;
            if (entry.signal != signal) continue;

            var player = Player.Instance;
            if (player == null) return;

            if (entry.itemToGive != null)
                entry.itemDiscardSpawner.HandleItemDiscarded(entry.itemToGive);

            if (entry.scoreToGive > 0)
                GameManager.Instance.AddScore(entry.scoreToGive);
        }
    }
}