using Assets.Scripts.Common;
using Combat.Damage;
using System.Collections;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    public GameObject target;
    public CommonAttackData attackData;

    public float timeInterval = 1.0f;

    private Coroutine dmgOverTime = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == target && 
            dmgOverTime == null)
        {
            dmgOverTime = StartCoroutine(Damage());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == target)
        {
            StopCoroutine(dmgOverTime);
            dmgOverTime = null;
        }
    }

    IEnumerator Damage()
    {
        while(true)
        {
            if (attackData.UseDamage)
            {
                var dgmReceiver = target.GetComponentInChildren<DamageReceiver>();
                if(dgmReceiver != null)
                {
                    dgmReceiver.Damage(new (attackData.damageAmount, gameObject));
                }
            }

            if (attackData.UseKnockBack)
            {
                var knockbackReceiver = target.GetComponentInChildren<KnockbackReceiver>();
                if (knockbackReceiver != null)
                {
                    knockbackReceiver.KnockBack(new Combat.Knockback.KnockBackData(
                        attackData.knockbackAngle, 
                        attackData.knockbackAmount, 
                        0, gameObject));
                }
            }

            if (attackData.UsePoise)
            {
                var poiseReceiver = target.GetComponentInChildren<PoiseDamageReceiver>();
                if (poiseReceiver != null)
                {
                    poiseReceiver.PoiseDamage(new PoiseDamageData(attackData.poiseAmount, gameObject));
                }
            }

            yield return new WaitForSeconds(timeInterval);
        }
    }
}
