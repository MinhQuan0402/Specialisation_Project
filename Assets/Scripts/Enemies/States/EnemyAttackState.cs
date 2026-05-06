using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyState", menuName = "State/Enemy State/Attack")]
public class EnemyAttackState : EnemyState
{
    private float attackTimer = 0.0f;
    private int combo = 1;
    public override void Init(Enemy enemy, EnemyData enemyData)
    {
        base.Init(enemy, enemyData);
        animBoolName = "attack";
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        Movement.SetVelocityZero();
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void Attack()
    {
        //if (KnockbackReceiver.IsStunActive) return;

        Core targetCore = Player.Instance.Core;
        if (!targetCore) return;
        KnockbackReceiver targetKnockbackReceiver = targetCore.GetComponent<KnockbackReceiver>();
        

        combo++;

        /*if (combo == 5)
        {
            targetKnockbackReceiver.Knockback(new Vector2(1.5f, 1f).normalized, 20, Movement.FacingDirection);
            targetKnockbackReceiver.PoiseDamage(1f);
            enemyData.attackCooldown = enemyData.maxCombo_attackCooldown;
            combo = 1;
        }
        else
        {
            targetKnockbackReceiver.Knockback(new Vector2(1.25f, 1f).normalized, 15, Movement.FacingDirection);
            targetKnockbackReceiver.PoiseDamage(0.5f);
            enemyData.attackCooldown = enemyData.default_attackCooldown;
        }*/

        attackTimer = enemyData.attackCooldown;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (attackTimer > 0.0f)
            attackTimer -= Time.deltaTime;

        if(!isExitingState)
        {
            // Detecting the player:
            Player player = Player.Instance;
            if (!player) return;

            // Check if the player can be seen:
            RaycastHit2D hit = Physics2D.Raycast(enemy.RB.position, (player.RB.position - enemy.RB.position).normalized, enemyData.playerDetectionRange, ~(1 << LayerMask.NameToLayer("Enemy")));
            if (hit && hit.transform == player.transform && (player.transform.position.y - enemy.RB.position.y) < enemyData.maxDetectionHeight)
            {
                float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
                if (dist <= enemyData.attackRange)
                {
                    // Check if the player is infront:
                    if (!Movement.IsInFront(player.transform.position))
                    {
                        Movement.Flip();
                    }
                    else if (attackTimer <= 0.0f)
                    {
                        Attack();
                    }
                }
                else // Charge the player if they are not in attack range
                {
                    stateMachine.ChangeState("charge");
                }
            }
            else
            {
                stateMachine.ChangeState("idle");
            } 
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
            
    }
}
