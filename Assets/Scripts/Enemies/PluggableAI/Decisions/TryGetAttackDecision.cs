using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/TryGetAttack")]
public class TryGetAttackDecision : EnemyDecision
{
    public override bool MakeDecision(EnemyController controller) => controller.TryGetAttack() != null;
}