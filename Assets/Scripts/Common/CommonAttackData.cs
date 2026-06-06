using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    [Serializable]
    public class CommonAttackData
    {
        public GameObject source;

        public bool UseDamage = true;
        public float damageAmount = 10.0f;

        public bool UseKnockBack = true;
        public float knockbackAmount = 10.0f;
        public Vector2 knockbackAngle = Vector2.one;

        public bool UsePoise = true;
        public float poiseAmount = 1f;
    }
}