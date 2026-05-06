using System;
using UnityEngine;

[Serializable]
public class AttackCharge : AttackData
{
    [field: SerializeField] public float ChargeTime { get; private set; }
    
    [field: SerializeField, Range(0, 1)] public int InitialChargeAmount { get; private set; }
    
    [field: SerializeField] public int NumOfCharges { get; private set; }
    
    [field: SerializeField] public GameObject ChargeIncreaseIndicatorParticlePrefab { get; private set; }
    
    [field: SerializeField] public GameObject FullyChargedIndicatorParticlePrefab { get; private set; }
    
    [field: SerializeField] public Vector2 ParticlesOffset { get; private set; }
}