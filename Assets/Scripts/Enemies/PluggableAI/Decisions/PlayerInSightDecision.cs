using System;
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
        bool result =  EnemyUtilities.PlayerInSight(controller,
                rangeType == RangeType.DetectionRange ?
                controller.Data.playerDetectionRange :
                controller.TryGetAttackRange(),
                controller.Data.whatIsObstacles);
        
        return result;
    }
}
