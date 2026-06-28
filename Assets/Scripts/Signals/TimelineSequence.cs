using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineInstance
{
    [field: SerializeField] public TimelineAsset Clip { get; private set; }
    [field: SerializeField] public DirectorWrapMode WrapMode { get; private set; }
    [field: SerializeField] public bool CanSkip { get; private set; } = false;
}

[CreateAssetMenu(fileName = "TimelineSequence", menuName = "Timeline/TimelineSequence")]
public class TimelineSequence : ScriptableObject
{
    [field: SerializeField] public List<TimelineInstance> Sequence {  get; private set; }
}
