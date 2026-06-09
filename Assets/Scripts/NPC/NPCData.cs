using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public enum NPCBehaviour { None, Dialogue, Shop, Cutscene, GiveItem }

[CreateAssetMenu(menuName = "NPC/Data")]
public class NPCData : ScriptableObject
{
    [Tooltip("Name of NPC")] public string NPCName;
    public Sprite NPCIcon;
    public NPCBehaviour behaviour;      // what happens on interact
}