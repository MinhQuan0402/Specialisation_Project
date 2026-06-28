using Combat.Damage;
using UnityEngine;

public class CombatTestDummy : MonoBehaviour
{
    private Animator anim;
    private CoreComp<DamageReceiver> damageReceiver;
    private CoreComp<Movement> movement;
    private CoreComp<CollisionSenses> collisionSenses;

    private Core Core;

    private void Awake()
    {
        Core = GetComponentInChildren<Core>();
        anim = GetComponentInChildren<Animator>();
        damageReceiver = new CoreComp<DamageReceiver>(Core);
        movement = new CoreComp<Movement>(GetComponentInChildren<Core>());
        collisionSenses = new CoreComp<CollisionSenses>(GetComponentInChildren<Core>());

        damageReceiver.Comp.OnTakingDamage += HandleTakingDamage;
    }

    private void Update()
    {
        Core.LogicUpdate();
    }

    private void HandleTakingDamage(GameObject source)
    {
        anim.SetBool("playerOnLeft", (Player.Instance.transform.position.x - transform.position.x) < 0);
        anim.SetTrigger("damage");
    }
}