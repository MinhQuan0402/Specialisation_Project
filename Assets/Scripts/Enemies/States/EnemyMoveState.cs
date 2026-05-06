using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "newEnemyState", menuName = "State/Enemy State/Move")]
public class EnemyMoveState : EnemyState
{
    protected bool isTouchingWall;
    protected bool isTouchingLedge;
    private float moveTime;

    public override void Init(Enemy enemy, EnemyData enemyData)
    {
        base.Init(enemy, enemyData);
        animBoolName = "move";
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingWall = CollisionSenses.WallFront;
        isTouchingLedge = CollisionSenses.LedgeVertical;
    }

    public override void Enter()
    {
        base.Enter();
        moveTime = Random.Range(enemyData.maxMoveTime*0.25f, enemyData.maxMoveTime);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(!isExitingState)
        {
            if (Time.time >= startTime + moveTime)
            {
                if (Random.value < 0.5f)
                    Movement.Flip();

                stateMachine.ChangeState("idle");
            } 
            else
            {
                Movement.SetVelocityX(enemyData.maxMoveVelocity * Movement.FacingDirection);

                if (isTouchingWall || !isTouchingLedge)
                {
                    Movement.Flip();
                    stateMachine.ChangeState("idle");
                } 
                else
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
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
            
    }
}
