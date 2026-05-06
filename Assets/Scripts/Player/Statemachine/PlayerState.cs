using UnityEngine;

public class PlayerState : State
{
    protected Player player;
    protected PlayerData playerData;

    private Movement movement;
    protected Movement Movement => movement ? movement : Core.GetCoreComponent(ref movement);

    private CollisionSenses collisionSenses;
    protected CollisionSenses CollisionSenses => collisionSenses ? collisionSenses : Core.GetCoreComponent(ref collisionSenses);

    public override void Init()
    {
        player = Player.Instance;
        playerData = player.PlayerData;
        stateMachine = player.StateMachine;
        anim = player.Anim;
        Core = player.Core;
        isExitingState = false;
    }
}
