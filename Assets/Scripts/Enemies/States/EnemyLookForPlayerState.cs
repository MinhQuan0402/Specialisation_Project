using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyState", menuName = "State/Enemy State/LookForPlayer")]
public class EnemyLookForPlayerState : EnemyState
{
    protected Vector2 lastPlayerPosition;
    private float lookTime = 0.0f;

    public override void Init(Enemy enemy, EnemyData enemyData)
    {
        base.Init(enemy, enemyData);
        animBoolName = "lookForPlayer";
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        lastPlayerPosition = Player.Instance.RB.position;
        lookTime = enemyData.maxLookingTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) return;

        if (lookTime > 0.0f)
        {
            lookTime -= Time.deltaTime;
            
            Player player = Player.Instance;
            if (!player) return;

            // Check if the player can be seen:
            RaycastHit2D hit = Physics2D.Raycast(enemy.RB.position, (player.RB.position - enemy.RB.position).normalized, enemyData.playerDetectionRange, ~(1 << LayerMask.NameToLayer("Enemy")));
            if (hit && hit.transform == player.transform && (player.transform.position.y - enemy.RB.position.y) < enemyData.maxDetectionHeight)
            {
                stateMachine.ChangeState("charge");
            }
            else
            {
                // Move to last player position:
                float dist = Vector3.Distance(lastPlayerPosition, enemy.transform.position);
                if (dist > 5f)
                {
                    if (!Movement.IsInFront(lastPlayerPosition))
                    {
                        Movement.Flip();
                    }

                    Movement.SetVelocityX(enemyData.maxMoveVelocity * Movement.FacingDirection);
                } 
                else
                {
                    Movement.SetVelocityZero();
                }
            }   
        } 
        else
        {
            stateMachine.ChangeState("idle");
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
