using UnityEngine;

[CreateAssetMenu(menuName = "Game/Step Data")]
public class StepData : ScriptableObject
{
    [Header("Identity"), Tooltip("Shown in editor / debug UI")]
    public string displayName;       
    [TextArea(2, 2), Tooltip("Optional [Only for me!]")]
    public string description;       

    [Header("Respawn"), Tooltip("Does this step define a spawn location?")]
    public bool hasRespawnPoint;   

    [Header("Hint"), Tooltip("Indexed by death count — tutorial feedback [Optional]")]
    public string[] deathHints;
}