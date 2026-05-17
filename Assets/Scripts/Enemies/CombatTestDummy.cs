using Combat.Damage;
using UnityEngine;

public class CombatTestDummy : MonoBehaviour, IDamageable
{
    private Animator anim;
    private CoreComp<Stats> stats;
    private CoreComp<Movement> movement;
    private CoreComp<CollisionSenses> collisionSenses;

    private float previousHealth = 0f;

    private Core Core;

    private void Awake()
    {
        Core = GetComponentInChildren<Core>();
        anim = GetComponentInChildren<Animator>();
        stats = new CoreComp<Stats>(Core);
        movement = new CoreComp<Movement>(GetComponentInChildren<Core>());
        collisionSenses = new CoreComp<CollisionSenses>(GetComponentInChildren<Core>());
        previousHealth = stats.Comp.Health.MaxValue;
    }

    private void Update()
    {
        if (stats.Comp.Health.CurrentValue <= 0)
        {
            stats.Comp.Health.Increase(stats.Comp.Health.MaxValue);
        }

        Core.LogicUpdate();

        if (previousHealth.Equals(stats.Comp.Health.CurrentValue)) return;
        anim.SetBool("playerOnLeft", (Player.Instance.transform.position.x - transform.position.x) < 0);
        anim.SetTrigger("damage");
        previousHealth = stats.Comp.Health.CurrentValue;
    }

    public void Damage(DamageData data) { }
}