using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite speakerIcon;
    [TextArea(2, 5)]
    public string text;
    public AudioClip voiceClip;
}

public class DialogueManager : SingletonPersistentTemplate<DialogueManager>
{
    [SerializeField] private float perCharDuration = 0.03f;

    public bool IsOpen { get; private set; } = false;

    public void ShowLine(DialogueLine line)
    {
        IsOpen = true;
        StartCoroutine(TypeLine(line));
    }

    public void EndDialogue()
    {
        IsOpen = false;
        UIManager.Instance.HideDialoguePrompt();
        StopAllCoroutines();
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        UIManager.Instance.EnableDialoguePrompt(line.speakerIcon, line.speakerName, "");
        var bodyText = UIManager.Instance.DialogueText;
        bodyText.text = "";
        foreach (char c in line.text)
        {
            bodyText.text += c;
            yield return new WaitForSecondsRealtime(perCharDuration);
        }
    }
}
