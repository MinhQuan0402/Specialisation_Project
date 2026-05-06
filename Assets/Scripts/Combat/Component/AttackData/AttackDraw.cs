using System;
using UnityEngine;

[Serializable]
public class AttackDraw : AttackData
{
    [field: SerializeField] public AnimationCurve DrawCurve { get; private set; }
        
    // The total time it takes to fully draw the bow -- If you want to calculate this based on frames it's drawTime = (1 / animationSampleRate) * numberOfFrames
    [field: SerializeField] public float DrawTime { get; private set; }
}