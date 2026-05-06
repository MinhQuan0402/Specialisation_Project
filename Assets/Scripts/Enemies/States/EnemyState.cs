using UnityEngine;

public class EnemyState : State
{
    protected Enemy enemy;
    protected EnemyData enemyData;

    private Movement movement;
    protected Movement Movement => movement ? movement : Core.GetCoreComponent(ref movement);

    private KnockbackReceiver _knockbackReceiver;
    protected KnockbackReceiver KnockbackReceiver => _knockbackReceiver ? _knockbackReceiver : Core.GetCoreComponent(ref _knockbackReceiver);

    private CollisionSenses colllisionSenses;
    protected CollisionSenses CollisionSenses => colllisionSenses ? colllisionSenses : Core.GetCoreComponent(ref colllisionSenses);

    public virtual void Init(Enemy enemy, EnemyData enemyData)
    {
        this.enemy = enemy;
        this.enemyData = enemyData;
        stateMachine = enemy.StateMachine;
        anim = enemy.Anim;
        Core = enemy.Core;
    }
}
