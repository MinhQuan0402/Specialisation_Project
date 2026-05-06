using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyState", menuName = "State/Enemy State/Idle")]
public class EnemyIdleState : EnemyState
{
    float idleDuration;
    protected bool isTouchingLedge;

    public override void Init(Enemy enemy, EnemyData enemyData)
    {
        base.Init(enemy, enemyData);
        animBoolName = "idle";
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isTouchingLedge = CollisionSenses.LedgeVertical;
    }

    public override void Enter()
    {
        base.Enter();
        idleDuration = Random.Range(enemyData.maxIdleElapsedTime * 0.25f, enemyData.maxIdleElapsedTime);
        Movement.SetVelocityZero();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if(Time.time >= startTime + idleDuration)
        {
            stateMachine.ChangeState("move");
        }
        else if (isTouchingLedge)
        {
            // Detecting the player:
            Player player = Player.Instance;
            if (!player) return;

            // Check if the player can be seen:
            RaycastHit2D hit = Physics2D.Raycast(enemy.RB.position, (player.RB.position - enemy.RB.position).normalized, enemyData.playerDetectionRange, ~(1 << LayerMask.NameToLayer("Enemy")));
            if (!hit || hit.transform != player.transform) return;

            // Check if the player is infront of the enemy:
            if (!Movement.IsInFront(player.transform.position)) return;

            // Check if the player is far on top of the enemy:
            if ((player.transform.position.y - enemy.RB.position.y) > enemyData.maxDetectionHeight) return;

            // Change state to PlayerDetectedState:
            stateMachine.ChangeState("playerDetected");
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
