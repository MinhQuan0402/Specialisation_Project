using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Tutorial,
    Hub,         
    Playing,
    Cutscene,
    Paused,
    LevelClear,
    BossFight,
    GameOver,     
    Victory       
}

public enum GameScene
{
    MainMenu,
    Tutorial,
    Hub,
    Level,       
    Boss
}

public enum DeathContext
{
    Tutorial,     
    Level,        
    Boss          
}

public class GameManager : SingletonPersistentTemplate<GameManager>
{
    // ── State ────────────────────────────────────────────────
    [field: SerializeField] public GameState CurrentState { get; private set; }
    [field: SerializeField] public GameScene CurrentScene { get; private set; }
    [field: SerializeField, ReadOnlyInspector] public DeathContext CurrentDeathContext { get; private set; }
        = DeathContext.Level; // safe default

    public event Action<GameState, GameState> OnStateChanged;
    public event Action<GameScene> OnSceneChanged;

    // ── Run data (resets on permadeath) ─────────────────────
    [field: SerializeField, ReadOnlyInspector] public int CurrentLevel { get; private set; } = 1;
    [field: SerializeField, ReadOnlyInspector]  public int RunScore { get; private set; } = 0;
    [field: SerializeField, ReadOnlyInspector]  public float RunTimeElapsed { get; private set; } = 0f;
    [field: SerializeField, ReadOnlyInspector]  public bool DeathThisRun { get; private set; } = false;

    // ── Persistent data (survives permadeath) ────────────────
    [field: SerializeField, ReadOnlyInspector] public int UpgradePoints { get; private set; } = 0;
    [field: SerializeField, ReadOnlyInspector] public int TotalRunsPlayed { get; private set; } = 0;
    [field: SerializeField, ReadOnlyInspector] public bool TutorialComplete { get; private set; } = false;

    private const int MaxLevels = 3; // levels before boss

    private void Start()
    {
        //ChangeState(TutorialComplete ? GameState.MainMenu : GameState.Tutorial);
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing || 
            CurrentScene == GameScene.Tutorial)
            RunTimeElapsed += Time.deltaTime;
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;
        GameState old = CurrentState;
        CurrentState = newState;
        HandleStateExit(old);
        HandleStateEnter(newState);
        OnStateChanged?.Invoke(old, newState);
    }

    private void HandleStateExit(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                Time.timeScale = 1f;
                break;
        }
    }

    private void HandleStateEnter(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                //LoadScene(GameScene.MainMenu);
                break;

            case GameState.Tutorial:
                Time.timeScale = 1f;
                //LoadScene(GameScene.Tutorial);
                break;

            case GameState.Hub:
                Time.timeScale = 1f;
                //LoadScene(GameScene.Hub);
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                //LoadScene(GameScene.Level);
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;

            case GameState.BossFight:
                Time.timeScale = 1f;
                //LoadScene(GameScene.Boss);
                break;

            case GameState.LevelClear:
                Time.timeScale = 0f;
                //HandleLevelClear();
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                HandlePermadeath();
                break;

            case GameState.Victory:
                Time.timeScale = 0f;
                //HandleVictory();
                break;
        }
    }

    private void HandlePermadeath()
    {
    }

    // ─────────────────────────────────────────────────────────
    // Public actions called by other scripts
    // ─────────────────────────────────────────────────────────
    public void StartRun(GameState startingState)
    {
        CurrentLevel = 1;
        RunScore = 0;
        RunTimeElapsed = 0f;
        DeathThisRun = false;
        ChangeState(startingState);
    }

    public void CompleteTutorial()
    {
        TutorialComplete = true;
        ChangeState(GameState.Hub);
    }

    public void SetDeathContext(DeathContext context)
    {
        CurrentDeathContext = context;
    }

    public void PlayerDied()
    {
        SceneEventBus.TriggerPlayerDied();
        if (CurrentScene == GameScene.Level)
            ChangeState(GameState.GameOver);
    }

    public void LevelCleared()
    {
        ChangeState(GameState.LevelClear);
    }

    /// <summary>Called from LevelClear UI — Next button.</summary>
    public void ProceedFromLevelClear()
    {
        CurrentLevel++;
        if (CurrentLevel > MaxLevels)
            ChangeState(GameState.BossFight);
        else
            ChangeState(GameState.Hub); // upgrade between levels
    }

    public void BossDefeated()
    {
        ChangeState(GameState.Victory);
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing) ChangeState(GameState.Paused);
        else if (CurrentState == GameState.Paused) ChangeState(GameState.Playing);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        HandlePermadeath();
        ChangeState(GameState.MainMenu);
    }

    public void AddScore(int scoreToAdd) => RunScore += scoreToAdd;

    private void LoadScene(GameScene scene)
    {
        if (CurrentScene == scene) return;
        CurrentScene = scene;
        OnSceneChanged?.Invoke(scene);
        SceneManager.LoadScene(scene.ToString());
    }
}
