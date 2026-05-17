using UnityEngine;

/// <summary>
/// Returns true when the player enters detectionRange.
/// Right-click → Create → PluggableAI2D → Decisions → PlayerInRange
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI2D/Decisions/PlayerInRange")]
public class PlayerInRangeDecision : EnemyDecision
{
    public override bool Decide(EnemyController controller) =>
        EnemyUtilities.PlayerInRange(controller, controller.Data.playerDetectionRange);
}