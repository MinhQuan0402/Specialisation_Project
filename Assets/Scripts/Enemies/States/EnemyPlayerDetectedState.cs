using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyState", menuName = "State/Enemy State/PlayerDetected")]
public class EnemyPlayerDetectedState : EnemyState
{
    public override void Init(Enemy enemy, EnemyData enemyData)
    {
        base.Init(enemy, enemyData);
        animBoolName = "playerDetected";
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

        if (isExitingState) return;

        Player player = Player.Instance;
        if (!player) return;

        // Check if the player can be seen:
        RaycastHit2D hit = Physics2D.Raycast(enemy.RB.position, (player.RB.position - enemy.RB.position).normalized, enemyData.playerDetectionRange, ~(1 << LayerMask.NameToLayer("Enemy")));
        if (hit && hit.transform == player.transform && (player.transform.position.y - enemy.RB.position.y) < enemyData.maxDetectionHeight)
        {
            float dist = Vector2.Distance(player.RB.position, enemy.RB.position);
            if (dist <= enemyData.attackRange) // Checks if the player is in attack range:
            {
                // Change to attack state
                stateMachine.ChangeState("attack");
            }
            else // Charge at the player is they are not within the attacking range:
            {
                stateMachine.ChangeState("charge");
            }
        } 
        else // The player cannot be seen
        {
            Movement.Flip();
            stateMachine.ChangeState("idle");
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
            
    }
}
