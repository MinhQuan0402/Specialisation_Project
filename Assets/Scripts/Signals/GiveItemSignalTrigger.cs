using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class GiveItemEntry
{
    public SignalAsset signal;
    public WeaponData weaponToGive;
    public int scoreToGive;
}


public class GiveItemSignalTrigger : MonoBehaviour
{
    [SerializeField] private List<GiveItemEntry> entries;

    public void OnNotify(SignalAsset signal)
    {
        foreach (var entry in entries)
        {
            if (entry.signal == null) continue;
            if (entry.signal != signal) continue;

            var player = Player.Instance;
            if (player == null) return;

            if (entry.weaponToGive != null)
                player.InventorySystem.TryToAddWeapon(entry.weaponToGive);

            if (entry.scoreToGive > 0)
                GameManager.Instance.AddScore(entry.scoreToGive);
        }
    }
}