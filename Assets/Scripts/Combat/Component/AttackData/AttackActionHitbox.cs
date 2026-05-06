using System;
using UnityEngine;

[Serializable]
public class AttackActionHitbox : AttackData
{
    public bool Debug;

    [field: SerializeField] public Rect Hitbox {  get; private set; }
}
