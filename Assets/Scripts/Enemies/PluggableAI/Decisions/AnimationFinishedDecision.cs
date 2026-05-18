using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AnimationFinished")]
public class AnimationFinishedDecision : EnemyDecision
{
    public override bool Decide(EnemyController controller) =>
        controller.isAnimationFinished;
}
