using UnityEngine;

/// <summary>
/// Walks (or flies) back and forth around the spawn point.
/// Right-click → Create → PluggableAI → Actions → Patrol
/// Works for both grounded and flying enemies (isFlying flag on EnemyStats).
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Actions/Move")]
public class MoveAction : EnemyAction
{
    private enum SpeedType
    {
        PatrolSpeed,
        ChaseSpeed
    }
    [SerializeField] private SpeedType speedType;

    public override void Act(EnemyController controller)
    {
        if (speedType == SpeedType.ChaseSpeed)
        {
            if (!EnemyUtilities.IsFacingAtPoint(controller, controller.lastSeenPlayerPoint))
            {
                controller.Movement.Flip();
            }
        }

        int direction = speedType == SpeedType.PatrolSpeed  ? controller.Movement.FacingDirection :
                        (controller.lastSeenPlayerPoint.x > controller.transform.position.x ? 1 : -1);
        controller.Movement.SetVelocityX(
           (speedType == SpeedType.PatrolSpeed ? 
           controller.Data.patrolSpeed :
           controller.Data.chaseSpeed) * direction
           );
    }
}
