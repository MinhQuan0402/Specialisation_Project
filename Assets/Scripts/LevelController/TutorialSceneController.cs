using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSceneController : BaseSceneController
{
    [SerializeField, ReadOnlyInspector] private int deathCount = 0;

    protected override void BuildRespawnMap()
    {
        // Populate the base class RespawnMap from our serialized list
        foreach (var entry in sceneSteps)
            if (entry.step != null)
                RespawnMap[entry.step] = entry;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //TutorialEventBus.OnMiniBossDefeated += HandleMiniBossDefeated;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //TutorialEventBus.OnMiniBossDefeated -= HandleMiniBossDefeated;
        GameManager.Instance.SetDeathContext(DeathContext.Level);
    }

    // ── Required overrides ────────────────────────────────────
    protected override void OnSceneReady()
    {
        GameManager.Instance.SetDeathContext(DeathContext.Tutorial);

        // Start at the first registered step
        if (sceneSteps.Count > 0)
            AdvanceToStep(sceneSteps[0].step);
    }

    protected override void HandlePlayerDied()
    {
        deathCount++;
        StartCoroutine(RespawnSequence());
    }

    protected override void OnStepChanged(StepData step)
    {
        deathCount = 0; // reset death count on each new step
    }

    // ── Zone override — any step zone advances the tutorial ───
    protected override void HandleZoneEntered(StepData step, GameObject who)
    {
        if (!IsStepRegistered(step)) return;
        AdvanceToStep(step);
    }

    // ── Private tutorial logic ────────────────────────────────
    private IEnumerator RespawnSequence()
    {
        float runTime = GameManager.Instance.RunTimeElapsed;
        int mins = (int)runTime / 60;
        int secs = (int)runTime % 60;
        string timerText = $"{mins:D2}:{secs:D2}";

        UIManager.Instance.EnableFeedbackPrompt(timerText, "");

        yield return StartCoroutine(TypeLine(GetHint()));

        RespawnAtStep(CurrentStep);

        /*if (CurrentStep == miniBossStep)
            miniBoss.ResetBoss();*/

        GameManager.Instance.StartRun(GameState.Tutorial);
        UIManager.Instance.HideFeedbackPrompt();
    }

    private IEnumerator TypeLine(string text)
    {
        var bodyText = UIManager.Instance.HintText;
        bodyText.text = "";
        foreach (char c in text)
        {
            bodyText.text += c;
            yield return new WaitForSecondsRealtime(0.03f);
        }

        yield return new WaitForSecondsRealtime(1.5f);
    }

    private string GetHint()
    {
        if (CurrentStep == null) return "Try again!";

        // Pull hints from the StepData asset itself
        if (CurrentStep.deathHints != null && CurrentStep.deathHints.Length > 0)
        {
            int index = Mathf.Clamp(deathCount - 1, 0,
                CurrentStep.deathHints.Length - 1);
            return CurrentStep.deathHints[index];
        }

        return "Try again!";
    }

    private void HandleMiniBossDefeated()
        => GameManager.Instance.CompleteTutorial();
}