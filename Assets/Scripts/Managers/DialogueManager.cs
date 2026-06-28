using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public CharacterData NPCData;

    [TextArea(2, 5)]
    public string text;
    public AudioClip voiceClip;
    public UnityEvent OnLineFinshed;
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

    private bool isLinePrinted;
    private bool isSkip = false;

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

    public void Skip()
    {
        if (!IsOpen)
        {
            Debug.LogWarning("Nothing to skip");
            return;
        }

        isSkip = true;
    }

    public void HandleDialogueControl(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isLinePrinted) Instance.Advance();
            else Instance.Skip();
        }
    }

    private void ShowCurrentLine()
    {
        var line = currentLines[currentIndex];
        StopAllCoroutines();
        isLinePrinted = false;
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
        UIManager.Instance.EnableSkipDialogueInstruction();
        UIManager.Instance.EnableDialoguePrompt(line.NPCData.CharacterIcon, line.NPCData.CharacterName, "");
        var bodyText = UIManager.Instance.DialogueText;
        bodyText.text = "\"";

        float currentDur = perCharDuration;
        foreach (char c in line.text)
        {
            bodyText.text += c;
            if (isSkip) currentDur = 0;
            yield return new WaitForSecondsRealtime(currentDur);
        }

        isSkip = false;
        isLinePrinted = true;
        bodyText.text += "\"";
        onLinePrinted?.Invoke();
        line.OnLineFinshed?.Invoke();
        UIManager.Instance.EnableContinueDialogueInstruction();
    }
}
