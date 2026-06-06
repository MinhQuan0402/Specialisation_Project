using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum TutorialStep
{
    Movement, 
    Dash,
    Spike,
    Combat
}

public static class TutorialEventBus
{
    public static event Action OnPlayerDiedInTutorial;

    public static void TriggerPlayerDiedInTutorial() => OnPlayerDiedInTutorial?.Invoke();
}

[System.Serializable]
public class RespawnPointData
{
    public string name;
    public TutorialStep step;
    public TutorialStepTrigger respawnPoint;

    [TextArea(1, 2)]
    public string stepLabel; // editor-only label so you know which is which
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RespawnPointData))]
public class RespawnPointDataDrawer : PropertyDrawer
{
    readonly float line = EditorGUIUtility.singleLineHeight;
    const float space = 2f;

    public override void OnGUI(
        Rect pos,
        SerializedProperty prop,
        GUIContent label)
    {
        Rect r = pos;
        r.height = line;

        EditorGUI.BeginProperty(pos, label, prop);

        prop.isExpanded = EditorGUI.Foldout(
            r,
            prop.isExpanded,
            label,
            true
        );


        if (prop.isExpanded)
        {
            r.y += EditorGUIUtility.singleLineHeight + 2;

            SerializedProperty step = prop.FindPropertyRelative("step");
            SerializedProperty name = prop.FindPropertyRelative("name");
            SerializedProperty respawnPoint = prop.FindPropertyRelative("respawnPoint");
            SerializedProperty stepLabel = prop.FindPropertyRelative("stepLabel");

            name.stringValue = step.enumDisplayNames[step.enumValueIndex];

            EditorGUI.PropertyField(r, step);
            r.y += EditorGUI.GetPropertyHeight(step) + 2;

            EditorGUI.PropertyField(r, respawnPoint);
            r.y += EditorGUI.GetPropertyHeight(respawnPoint) + 2;

            EditorGUI.PropertyField(r, stepLabel);
            r.y += EditorGUI.GetPropertyHeight(stepLabel) + 2;
        }
    }

    public override float GetPropertyHeight(
        SerializedProperty prop,
        GUIContent label)
    {
        if (!prop.isExpanded)
            return line;

        float height = (line + space) * 2;
        height += (line + space) * 2;
        height += (line + space) * 2;
        return height;
    }
}
#endif

public class TutorialSceneController : SingletonTemplate<TutorialSceneController>
{
    [Header("Respawn Points")]
    [Tooltip("assign in Inspector, order matches TutorialStep enum")]
    [SerializeField] private List<RespawnPointData> respawnPoints;

    //[SerializeField] private Transform       miniBossRespawnPoint;

    [Header("UI Feedback")]
    [SerializeField] private GameObject      deathFeedbackPanel; // "Try again!" overlay
    [SerializeField] private TMPro.TextMeshProUGUI attemptsText;
    
    [field: SerializeField, ReadOnlyInspector] public TutorialStep CurrentStep { get; private set; } = TutorialStep.Movement;

    private Player player;
    private int    deathCount = 0;

    private readonly Dictionary<TutorialStep, RespawnPointData> respawnMap = new();

    protected override void Awake()
    {
        base.Awake();

        // Build the lookup dictionary from the serialized list
        foreach (var entry in respawnPoints)
        {
            respawnMap[entry.step] = entry;
        }
    }

    private void Start()
    {
        player = Player.Instance;

        GameManager.Instance.SetDeathContext(DeathContext.Tutorial);

        TutorialEventBus.OnPlayerDiedInTutorial += HandleTutorialDeath;

        AdvanceToStep(TutorialStep.Movement);
    }

    private void OnDestroy()
    {
        TutorialEventBus.OnPlayerDiedInTutorial -= HandleTutorialDeath;
        GameManager.Instance.SetDeathContext(DeathContext.Level);
    }

    // ── Step progression ──────────────────────────────────────
    public void AdvanceToStep(TutorialStep newStep)
    {
        CurrentStep = newStep;
        deathCount = 0; // reset death count per step
    }

    // ── Death handling ────────────────────────────────────────
    private void HandleTutorialDeath()
    {
        deathCount++;
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        // 1. Show death feedback
        //ShowDeathFeedback();

        // 2. Brief pause for feel
        yield return new WaitForSecondsRealtime(1.2f);

        // 3. Respawn player
        RespawnAtCurrentStep();
        //RespawnMiniBoss();

        // 4. Hide feedback
        //HideDeathFeedback();
    }

    private void RespawnAtCurrentStep()
    {
        if (!respawnMap.TryGetValue(CurrentStep, out var data)) return;
        player.Respawn(data.respawnPoint.SpawnPoint);
    }

    /*private void RespawnMiniBoss()
    {
        //miniBoss.transform.position = miniBossRespawnPoint.position;
        //miniBoss.ResetBoss(); // resets HP, AI state, pattern
    }*/

    private void ShowDeathFeedback()
    {
        deathFeedbackPanel.SetActive(true);

        string msg = deathCount switch
        {
            1 => "Watch the attack pattern...",
            2 => "Try dodging when it charges!",
            _ => "You can do this!"
        };

        attemptsText.text = msg;
    }

    private void HideDeathFeedback()
    {
        deathFeedbackPanel.SetActive(false);
    }

    // ── Mini boss cleared ─────────────────────────────────────
    /*private void HandleMiniBossDefeated()
    {
        //GameManager.Instance.CompleteTutorial(); // → Hub
    }*/
}