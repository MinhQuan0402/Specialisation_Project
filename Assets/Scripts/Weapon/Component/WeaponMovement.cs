using UnityEngine;

public class WeaponMovement : WeaponComponent<WeaponMovementData, AttackMovement>
{
    private CoreComp<Movement> movementCore;

    private void HandleStartMovement()
    {
        Vector2 attackDirection = new Vector2(currentAttackData.Direction.x * movementCore.Comp.FacingDirection, currentAttackData.Direction.y);
        movementCore.Comp.AddVelocity(currentAttackData.Velocity, attackDirection);
    }

    private void HandleStopMovement()
    {
        movementCore.Comp.SetVelocityX(0f);
    }

    protected override void Start()
    {
        base.Start();
        
        eventHandler.OnStartMovement += HandleStartMovement;
        eventHandler.OnStopMovement += HandleStopMovement;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        eventHandler.OnStartMovement -= HandleStartMovement;
        eventHandler.OnStopMovement -= HandleStopMovement;
    }

    protected override void HandleEnter()
    {
        base.HandleEnter();

        movementCore ??= new CoreComp<Movement>(Core);
    }
}
