using UnityEngine;

/// <summary>
/// Returns true when the player enters detectionRange.
/// Right-click → Create → PluggableAI → Decisions → PlayerInRange
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decisions/PlayerInRange")]
public class PlayerInRangeDecision : EnemyDecision
{
    private enum RangeType
    {
        DetectionRange,
        AttackRange 
    }

    [SerializeField] private RangeType rangeType;

    public override bool MakeDecision(EnemyController controller) =>
        EnemyUtilities.PlayerInRange(controller, 
                                     rangeType == RangeType.DetectionRange ? 
                                     controller.Data.playerDetectionRange : 
                                     controller.Data.attackRange);
}