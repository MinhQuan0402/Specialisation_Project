using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Flip")]
public class FlipAction : EnemyAction
{
    public override void Act(EnemyController controller)
    {
        controller.Movement.Flip();
    }
}
