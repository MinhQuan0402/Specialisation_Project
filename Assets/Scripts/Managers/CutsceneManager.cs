using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneManager : SingletonPersistentTemplate<CutsceneManager>
{
    //[SerializeField] private CanvasGroup fadeOverlay;

    public bool IsPlaying { get; private set; }

    private PlayableDirector director;
    private Action onComplete;

    private int currentClipIndex = 0;
    private TimelineSequence currentSquence = null;

    private void Start()
    {
        director = GetComponent<PlayableDirector>();
    }

    // Play a Timeline cutscene via its PlayableDirector in the scene
    public void Play(TimelineSequence newSequence, Action onFinished = null)
    {
        if (IsPlaying || newSequence == null || currentSquence != null) return;
        if (newSequence.Sequence.Count == 0) return;

        currentClipIndex = 0;
        currentSquence = newSequence;
        onComplete = onFinished;

        if (newSequence.Sequence.Count == 1)
            director.stopped += OnDirectorStopped;

        director.playableAsset = newSequence.Sequence[currentClipIndex].Clip;
        director.extrapolationMode = newSequence.Sequence[currentClipIndex].WrapMode;
        StartCoroutine(StartCutscene());
    }

    public void AdvanceSequence()
    {
        if (!IsPlaying || currentSquence == null) return;

        currentClipIndex++;

        if (currentClipIndex == currentSquence.Sequence.Count - 1)
            director.stopped += OnDirectorStopped;

        director.playableAsset = currentSquence.Sequence[currentClipIndex].Clip;
        director.extrapolationMode = currentSquence.Sequence[currentClipIndex].WrapMode;
        director.Play();
    }


    public void Skip()
    {
        if (!IsPlaying || director == null) return;
        if (!currentSquence.Sequence[currentClipIndex].CanSkip) return;

        director.time = director.duration; // jump to end
        director.Evaluate();
        director.Stop();

        if (currentClipIndex < currentSquence.Sequence.Count - 1) 
            AdvanceSequence();
    }

    // ── Internal ──────────────────────────────────────────────
    private IEnumerator StartCutscene()
    {
        IsPlaying = true;
        GameManager.Instance.ChangeState(GameState.Cutscene);
        CutsceneEventBus.TriggerCutsceneStarted(director.playableAsset.name);
        UIManager.Instance.ActivateCinematicBar(0.5f);
        Player.Instance.Paused();

        director.Play();
        yield return null;
    }

    private void OnDirectorStopped(PlayableDirector director)
    {
        if (!gameObject.activeSelf) return;
        director.stopped -= OnDirectorStopped;
        StartCoroutine(EndCutscene());
    }

    private IEnumerator EndCutscene()
    {
        Player.Instance.Unpaused();
        IsPlaying = false;
        GameManager.Instance.ChangeState(GameState.Playing);
        CutsceneEventBus.TriggerCutsceneEnded(director.playableAsset.name);
        UIManager.Instance.DeactivateCinematicBar(0.5f);
        onComplete?.Invoke();
        onComplete = null;
        yield return null;
    }
}