using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
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
        foreach (var entry in entries)
        {
            if (entry.signal == null) continue;
            if (entry.signal == signal)
            {
                CutsceneManager.Instance.AdvanceSequence();
                DialogueManager.Instance.StartSequence(entry.dialogueSequence, 
                                                      onDone: HandleOnDone);
                return;
            }
        }
    }

    private void HandleOnDone()
    {
        CutsceneManager.Instance.AdvanceSequence();
    }
}
