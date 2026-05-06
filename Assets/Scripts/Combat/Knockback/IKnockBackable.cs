using UnityEngine;

namespace Combat.Knockback
{
    public interface IKnockBackable
    {
        void KnockBack(KnockBackData data);
    }
}