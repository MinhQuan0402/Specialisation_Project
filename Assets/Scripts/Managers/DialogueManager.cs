using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public NPCData NPCData;

    [TextArea(2, 5)]
    public string text;
    public AudioClip voiceClip;
}

[System.Serializable]
public class DialogueSequence
{
    public string sequenceID;
    public List<DialogueLine> lines;
}

public class DialogueManager : SingletonPersistentTemplate<DialogueManager>
{
    [SerializeField] private float perCharDuration = 0.03f;

    public bool IsOpen { get; private set; } = false;

    private List<DialogueLine> currentLines;
    private int currentIndex;
    private Action onComplete;
    private Action onLinePrinted;

    public void StartSequence(DialogueSequence sequence, Action onDone = null, Action onLineFinished = null)
    {
        currentLines = sequence.lines;
        currentIndex = 0;
        onComplete = onDone;
        onLinePrinted = onLineFinished;
        IsOpen = true;
        ShowCurrentLine();
    }

    public void Advance()
    {
        currentIndex++;
        if (currentIndex >= currentLines.Count)
            EndDialogue();
        else
            ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        var line = currentLines[currentIndex];
        StopAllCoroutines();
        StartCoroutine(TypeLine(line));
    }

    public void EndDialogue()
    {
        IsOpen = false;
        UIManager.Instance.HideDialoguePrompt();
        StopAllCoroutines();
        onComplete?.Invoke();
        onComplete = null;
        onLinePrinted = null;
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        UIManager.Instance.EnableDialoguePrompt(line.NPCData.NPCIcon, line.NPCData.NPCName, "");
        var bodyText = UIManager.Instance.DialogueText;
        bodyText.text = "\"";
        foreach (char c in line.text)
        {
            bodyText.text += c;
            yield return new WaitForSecondsRealtime(perCharDuration);
        }

        bodyText.text += "\"";
        onLinePrinted?.Invoke();
    }
}
