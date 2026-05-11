using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class AttackBlock : AttackData
{
    [field: SerializeField] public DirectionalInformation[] BlockDirectionalInformation { get; private set; }

    [field: SerializeField] public PhaseTime BlockWindowStart { get; private set; }

    [field: SerializeField] public PhaseTime BlockWindowEnd { get; private set; }

    [field: SerializeField] public GameObject Particles { get; private set; }

    [field: SerializeField] public Vector2 ParticleOffset { get; private set; }

    public bool IsBlocked(float angle, out DirectionalInformation directionalInformation)
    {
        directionalInformation = null;

        foreach (DirectionalInformation blockDirectionalInformation in BlockDirectionalInformation)
        {
            var blocked = blockDirectionalInformation.IsAngleBetween(angle);

            if (!blocked) continue;

            directionalInformation = blockDirectionalInformation;

            return true;
        }

        return false;
    }
}
