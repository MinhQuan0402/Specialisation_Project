using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyState", menuName = "State/Enemy State/Charge")]
public class EnemyChargeState : EnemyState
{
    protected bool isTouchingLedge;

    public override void Init(Enemy enemy, EnemyData enemyData)
    {
        base.Init(enemy, enemyData);
        animBoolName = "charge";
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingLedge = CollisionSenses.LedgeVertical;
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
            float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (dist <= enemyData.attackRange) // Checks if the player is in attack range
            {
                // Change to attack state
                stateMachine.ChangeState("attack");
            }
            else // Charge at the player is they are not within the attacking range:
            {
                if (!isTouchingLedge)
                {
                    stateMachine.ChangeState("idle");
                }
                else if (Movement.IsInFront(player.transform.position)) // Check if the player is infront of the enemy
                {
                    Movement.SetVelocityX(enemyData.maxMoveVelocity * Movement.FacingDirection);
                }
                else
                {
                    Movement.Flip();
                }
            }
        }
        else
        {
            stateMachine.ChangeState("lookForPlayer");
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();   
    }
}
