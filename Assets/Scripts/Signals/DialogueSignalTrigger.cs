using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

[System.Serializable]
public class DialogueEntry
{
    public SignalAsset signal;
    public DialogueSequence dialogueSequence;
}

[RequireComponent(typeof(PlayableDirector))]
public class DialogueSignalTrigger : MonoBehaviour
{
    [SerializeField] private List<DialogueEntry> entries;

    public void OnNotify(SignalAsset signal)
    {
        Debug.Log("Player");
        foreach (var entry in entries)
        {
            if (entry.signal == null) continue;
            if (entry.signal == signal)
            {
                var director = GetComponent<PlayableDirector>();
                director.playableGraph.GetRootPlayable(0).SetSpeed(0);
                DialogueManager.Instance.StartSequence(entry.dialogueSequence, 
                                                      onDone: Resume, 
                                                      () => Invoke(nameof(AdvanceDialogue), 1.0f));
                return;
            }
        }
    }

    private void AdvanceDialogue()
    {
        DialogueManager.Instance.Advance();
    }

    private void Resume()
    {
        var director = GetComponent<PlayableDirector>();
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }
}
