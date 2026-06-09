using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(StepPointData))]
public class StepPointDataDrawer : PropertyDrawer
{
    readonly float line = EditorGUIUtility.singleLineHeight;
    const float space = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect r = position;
        r.height = line;

        EditorGUI.BeginProperty(position, label, property);
        SerializedProperty stepData = property.FindPropertyRelative("step");

        EditorGUI.PropertyField(r, stepData);
        r.y += EditorGUI.GetPropertyHeight(stepData) + 2;

        GUIContent uIContent = new GUIContent("Components");
        property.isExpanded = EditorGUI.Foldout(
            r,
            property.isExpanded,
            uIContent,
            true
        );

        if (property.isExpanded &&
            stepData.objectReferenceValue != null && 
            ((StepData)stepData.objectReferenceValue).hasRespawnPoint)
        {
            r.y += line + space;

            SerializedProperty respawnPoint = property.FindPropertyRelative("respawnPoint");
            EditorGUI.PropertyField(r, respawnPoint);
            r.y += EditorGUI.GetPropertyHeight(respawnPoint) + 2;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = (line + space) * 2;

        SerializedProperty stepData = property.FindPropertyRelative("step");
        if (property.isExpanded &&
            stepData.objectReferenceValue != null && 
            ((StepData)stepData.objectReferenceValue).hasRespawnPoint)
        {
            height += line + space;
        }

        return height;
    }
}
#endif

[System.Serializable]
public class StepPointData
{
    public StepData step;
    public Transform respawnPoint;
}

public abstract class BaseSceneController : SingletonTemplate<BaseSceneController>
{
    [Header("Scene Steps")]
    [SerializeField] protected List<StepPointData> sceneSteps;

    // Runtime lookup — built from the serialized list
    protected Dictionary<StepData, StepPointData> RespawnMap { get; private set; }

    [field: SerializeField, ReadOnlyInspector] public StepData CurrentStep { get; protected set; }

    // ── Lifecycle ─────────────────────────────────────────────
    protected override void Awake()
    {
        base.Awake();

        RespawnMap = new Dictionary<StepData, StepPointData>();
        BuildRespawnMap();
    }

    protected virtual void OnEnable()
    {
        ZoneEventBus.OnZoneEntered += HandleZoneEntered;
        ZoneEventBus.OnZoneExited += HandleZoneExited;
        //NPCEventBus.OnNPCInteracted += HandleNPCInteracted;
        SceneEventBus.OnPlayerDied += HandlePlayerDied;
        //CutsceneEventBus.OnCutsceneEnded += HandleCutsceneEnded;
    }

    protected virtual void OnDisable()
    {
        ZoneEventBus.OnZoneEntered -= HandleZoneEntered;
        ZoneEventBus.OnZoneExited -= HandleZoneExited;
        //NPCEventBus.OnNPCInteracted -= HandleNPCInteracted;
        SceneEventBus.OnPlayerDied -= HandlePlayerDied;
        //CutsceneEventBus.OnCutsceneEnded -= HandleCutsceneEnded;
    }

    protected virtual void Start() => OnSceneReady();

    // ── Step management ───────────────────────────────────────
    protected void AdvanceToStep(StepData step)
    {
        if (step == null || !ContainStep(step))
        {
            Debug.LogWarning($"Step {step?.displayName} not registered in {gameObject.name}");
            return;
        }

        CurrentStep = step;
        OnStepChanged(step);
    }

    // Called when step changes — subclasses react
    protected virtual void OnStepChanged(StepData step) { }

    // Validate — warn if a ZoneTrigger in scene uses an unregistered step
    protected bool IsStepRegistered(StepData step) => ContainStep(step);

    // ── Abstract ──────────────────────────────────────────────
    protected abstract void OnSceneReady();
    protected abstract void HandlePlayerDied();

    // ── Virtual ───────────────────────────────────────────────
    protected virtual void HandleZoneEntered(StepData step, GameObject who) { }
    protected virtual void HandleZoneExited(StepData step, GameObject who) { }
    //protected virtual void HandleNPCInteracted(NPCID npcID) { }
    protected virtual void HandleCutsceneEnded(string cutsceneID) { }

    // ── Shared utilities ──────────────────────────────────────
    protected virtual void BuildRespawnMap() { }  // subclass populates if needed

    protected void RespawnPlayer(Vector3 position)
    {
        var player = Player.Instance;
        if (player == null) return;
        player.Respawn(position);
    }

    protected void RespawnAtStep(StepData step)
    {
        if (RespawnMap.TryGetValue(step, out var data))
            RespawnPlayer(data.respawnPoint.position);
        else
            Debug.LogWarning($"No respawn point for step: {step?.displayName}");
    }

    protected void SetPlayerInput(bool enabled)
    {
        if (enabled) Player.Instance.Freeze(); else Player.Instance.UnFreeze();
    }

    protected bool ContainStep(StepData step)
    {
        foreach (var sceneStep in sceneSteps)
            if (sceneStep.step == step) return true;
        return false;
    }
}