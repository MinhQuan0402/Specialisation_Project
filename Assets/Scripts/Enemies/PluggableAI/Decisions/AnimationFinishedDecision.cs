using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AnimationFinished")]
public class AnimationFinishedDecision : EnemyDecision
{
    public override bool MakeDecision(EnemyController controller) =>
        controller.isAnimationFinished;
}
