using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AltarGate : Structure
{
    [SerializeField] private List<SpriteRenderer> letters;
    [SerializeField] private GameObject altarCam;

    [SerializeField] private float animationSpeed = 1.0f;
    [SerializeField] private AnimationCurve letterAnimationCurve;

    [SerializeField] private DialogueSequence missingLetterDialogue;
    [SerializeField] private DialogueSequence lettersUnlockedDialogue;

    public UnityEvent OnAltarCompleted;

    public int MaxLetter => letters.Count;

    [field: SerializeField, ReadOnlyInspector, Header("Debug")]
    public int CurrentUnlockLetter { get; private set; } = 0;

    public override void OnInteract()
    {
        if (InInteraction) return;

        base.OnInteract();
        UnlockLetter();
    }

    private void UnlockLetter()
    {
        int totalKeys = Player.Instance.InventorySystem.TotalKey;
        StartCoroutine(UnlockLetterProcedural(totalKeys));
    }

    IEnumerator UnlockLetterProcedural(int currentNumKeys)
    {
        altarCam.SetActive(true);
        Player.Instance.Paused();
        UIManager.Instance.ActivateCinematicBar(0.3f);
        yield return new WaitForSeconds(2.0f);

        while (CurrentUnlockLetter < currentNumKeys)
        {
            yield return StartCoroutine(LetterAnimation());
            CurrentUnlockLetter++;
        }

        yield return StartCoroutine(LetterUnlockCheck());

        altarCam.SetActive(false);
        UIManager.Instance.DeactivateCinematicBar(0.3f);
        Player.Instance.Unpaused();
        TriggerCompletedEvent();
        yield return null;
    }

    IEnumerator LetterAnimation()
    {
        Color letterColor = letters[CurrentUnlockLetter].color;
        float startTime = Time.time;
        while (letterColor.a < 1.0f)
        {
            letterColor.a = Mathf.Lerp(0.0f, 1.0f, letterAnimationCurve.Evaluate((Time.time - startTime) * animationSpeed));
            letters[CurrentUnlockLetter].color = letterColor;
            yield return null;
        }

        letterColor.a = 1.0f;
        letters[CurrentUnlockLetter].color = letterColor;
        yield return null;
    }

    private IEnumerator LetterUnlockCheck()
    {
        if (CurrentUnlockLetter != MaxLetter)
        {
            DialogueManager.Instance.StartSequence(missingLetterDialogue);
        }
        else
        {
            DialogueManager.Instance.StartSequence(lettersUnlockedDialogue);
            OnAltarCompleted?.Invoke();
        }

        UIManager.Instance.InputActions.Player.Interact.started += DialogueManager.Instance.HandleDialogueControl;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsOpen);
        UIManager.Instance.InputActions.Player.Interact.started -= DialogueManager.Instance.HandleDialogueControl;

        yield return null;
    }
}