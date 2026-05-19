using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/PlayerInSight")]
public class PlayerInSightDecision : EnemyDecision
{
    private enum RangeType
    {
        DetectionRange,
        AttackRange
    }

    [SerializeField] private RangeType rangeType;

    public override bool MakeDecision(EnemyController controller)
    {
        return  EnemyUtilities.PlayerInSight(controller,
                rangeType == RangeType.DetectionRange ?
                controller.Data.playerDetectionRange :
                controller.Data.attackRange,
                controller.Data.whatIsObstacles);
    }
}
