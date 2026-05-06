using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyState", menuName = "State/Enemy State/Stun")]
public class EnemyStunState : EnemyState
{
    public override void Init(Enemy enemy, EnemyData enemyData)
    {
        base.Init(enemy, enemyData);
        animBoolName = "stun";
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        /*if (!KnockbackReceiver.IsStunActive)
        {
            stateMachine.ChangeState("lookForPlayer");
        }*/
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
