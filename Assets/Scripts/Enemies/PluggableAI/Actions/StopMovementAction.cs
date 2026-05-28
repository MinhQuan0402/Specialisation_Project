using Pathfinding;
using UnityEngine;

/// <summary>
/// Stops all movement. Used in IdleState and HurtState.
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Actions/Stop Movement")]
public class StopMovementAction : EnemyAction
{
    public override void Act(EnemyController controller)
    {
        controller.Movement.SetVelocityX(0.0f);

        if(controller.Data.isFlying)
        {
            controller.DestinationSetter.target = null;
            controller.AIPath.SetPath(null);
        }
    }
}