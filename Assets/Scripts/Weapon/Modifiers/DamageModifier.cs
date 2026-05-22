using UnityEngine;
using System.Collections;
using Combat.Damage;
using System;

public class DamageModifier : Modifier<DamageData>
{
    // Event that fires off if the block was actually successful.
    public event Action<GameObject> OnModified;

    // The function that we call to determine if a block was sucessful.
    private readonly ConditionalDelegate isBlocked;

    public DamageModifier(ConditionalDelegate isBlocked)
    {
        this.isBlocked = isBlocked;
    }


    public override DamageData ModifyValue(DamageData value)
    {
        if (isBlocked(value.Source.transform, out var blockDirectionalInformation))
        {
            value.SetAmount(value.Amount * (1 - blockDirectionalInformation.DamageAbsorption));
            Debug.Log(value.Source);
            OnModified?.Invoke(value.Source);
        }

        return value;
    }
}