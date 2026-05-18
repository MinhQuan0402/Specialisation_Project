using UnityEngine;

/// <summary>
/// Returns true when the player enters detectionRange.
/// Right-click → Create → PluggableAI → Decisions → PlayerInRange
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Decisions/PlayerInRange")]
public class PlayerInRangeDecision : EnemyDecision
{
    public override bool Decide(EnemyController controller) =>
        EnemyUtilities.PlayerInRange(controller, controller.Data.playerDetectionRange);
}