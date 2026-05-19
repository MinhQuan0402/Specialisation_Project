using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/LookAtPlayer")]
public class LookAtPlayerAction : EnemyAction
{
    public override void Act(EnemyController controller) =>
        EnemyUtilities.FacePlayer(controller);
}
