using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public enum NPCBehaviour { None, Dialogue, Shop, Cutscene, GiveItem }

[CreateAssetMenu(menuName = "NPC/Data")]
public class CharacterData : ScriptableObject
{
    [Tooltip("Name of Character")] public string CharacterName;
    public Sprite CharacterIcon;
    public NPCBehaviour behaviour;      // what happens on interact
}