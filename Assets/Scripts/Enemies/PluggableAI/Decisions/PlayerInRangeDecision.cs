using UnityEngine;

public class PlayerInRangeDecision : EnemyDecision
{
    public override bool Decide(EnemyController controller) =>
        EnemyUtilities.PlayerInRange(controller, controller.Data.playerDetectionRange);
}
