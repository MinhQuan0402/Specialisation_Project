using System;
using UnityEngine;

[Serializable]
public class AttackTargeter : AttackData
{
    [field: SerializeField] public Rect Area { get; private set; }
    [field: SerializeField] public LayerMask DamageableLayers { get; private set; }
}