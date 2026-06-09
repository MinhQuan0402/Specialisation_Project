using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections;

public class CutsceneManager : SingletonPersistentTemplate<CutsceneManager>
{
    //[SerializeField] private CanvasGroup fadeOverlay;

    public bool IsPlaying { get; private set; }

    private PlayableDirector activeDirector;
    private Action onComplete;

    // Play a Timeline cutscene via its PlayableDirector in the scene
    public void Play(PlayableDirector director, Action onFinished = null)
    {
        if (IsPlaying) return;

        activeDirector = director;
        onComplete = onFinished;

        director.stopped += OnDirectorStopped;
        StartCoroutine(StartCutscene(director));
    }

    public void Skip()
    {
        if (!IsPlaying || activeDirector == null) return;
        activeDirector.time = activeDirector.duration; // jump to end
        activeDirector.Evaluate();
        activeDirector.Stop();
    }

    // ── Internal ──────────────────────────────────────────────
    private IEnumerator StartCutscene(PlayableDirector director)
    {
        IsPlaying = true;
        GameManager.Instance.ChangeState(GameState.Cutscene);
        CutsceneEventBus.TriggerCutsceneStarted(director.playableAsset.name);
        UIManager.Instance.SetHUDActive(false);
        Player.Instance.Freeze();

        director.Play();
        yield return null;
    }

    private void OnDirectorStopped(PlayableDirector director)
    {
        director.stopped -= OnDirectorStopped;
        StartCoroutine(EndCutscene(director));
    }

    private IEnumerator EndCutscene(PlayableDirector director)
    {
        Player.Instance.UnFreeze();
        IsPlaying = false;
        GameManager.Instance.ChangeState(GameState.Playing);
        CutsceneEventBus.TriggerCutsceneEnded(director.playableAsset.name);
        UIManager.Instance.SetHUDActive(true);
        onComplete?.Invoke();
        onComplete = null;
        yield return null;
    }
}