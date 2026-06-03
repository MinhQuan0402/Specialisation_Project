using UnityEngine;
using Combat.Knockback;

public class KnockbackReceiver : CoreComponent, IKnockBackable
{
    [SerializeField] private float maxKnockbackTime = 0.2f;
    
    public Modifiers<Modifier<KnockBackData>, KnockBackData> Modifiers { get; } = new();

    private CoreComp<Movement> movement;
    private CoreComp<CollisionSenses> collisionSenses;
    
    private bool isKnockbackActive;
    private float knockbackStartTime;

    public override void LogicUpdate()
    {
        CheckKnockback();
    }

    public void KnockBack(KnockBackData data)
    {
        data = Modifiers.ApplyAllModifiers(data);

        movement.Comp.SetVelocity(data.Strength, data.Angle, data.Direction);
        movement.Comp.CanSetVelocity = false;
        isKnockbackActive = true;
        knockbackStartTime = Time.time;
    }
    

    private void CheckKnockback()
    {
        if(isKnockbackActive 
           && ((movement.Comp.CurrentVelocity.y <= 0.01f 
           && collisionSenses.Comp.GroundCheck != null 
           && collisionSenses.Comp.Grounded)
               || (Time.time >= knockbackStartTime + maxKnockbackTime)))
        {
            isKnockbackActive = false;
            movement.Comp.CanSetVelocity = true;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        movement = new CoreComp<Movement>(core);
        collisionSenses = new CoreComp<CollisionSenses>(core);
    }
}


