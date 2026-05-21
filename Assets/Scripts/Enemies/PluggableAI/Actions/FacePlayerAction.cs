using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "PluggableAI/Actions/FacePlayer")]
public class FacePlayerAction : EnemyAction
{
    public override void Act(EnemyController controller) =>
        EnemyUtilities.FacePlayer(controller);
}