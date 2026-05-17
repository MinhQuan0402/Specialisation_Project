using UnityEngine;

/// <summary>
/// Walks (or flies) back and forth around the spawn point.
/// Right-click → Create → PluggableAI2D → Actions → Patrol
/// Works for both grounded and flying enemies (isFlying flag on EnemyStats).
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI2D/Actions/Patrol")]
public class PatrolAction : EnemyAction
{
    public override void Act(EnemyController controller)
    {
        int direction = controller.Movement.FacingDirection;
        controller.Movement.SetVelocityX(controller.Data.maxMoveVelocity * direction);
    }
}
