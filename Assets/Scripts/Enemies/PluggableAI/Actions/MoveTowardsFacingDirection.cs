using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MoveTowardsFacingDirection")]
public class MoveTowardsFacingDirection : EnemyAction
{
    private enum SpeedType
    {
        PatrolSpeed,
        ChaseSpeed
    }
    [SerializeField] private SpeedType speedType;

    public override void Act(EnemyController controller) =>
        controller.Movement.SetVelocityX((speedType == SpeedType.PatrolSpeed ? 
            controller.Data.patrolSpeed : controller.Data.chaseSpeed) * controller.Movement.FacingDirection);
}