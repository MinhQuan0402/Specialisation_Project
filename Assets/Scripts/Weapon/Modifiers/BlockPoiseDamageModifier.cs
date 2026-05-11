using UnityEngine;
using System.Collections;

public class BlockPoiseDamageModifier : Modifier<PoiseDamageData>
{
    private readonly ConditionalDelegate isBlocked;

    public BlockPoiseDamageModifier(ConditionalDelegate isBlocked)
    {
        this.isBlocked = isBlocked;
    }

    public override PoiseDamageData ModifyValue(PoiseDamageData value)
    {
        if (isBlocked(value.Source.transform, out var blockDirectionalInformation))
        {
            value.SetAmount(value.Amount * (1 - blockDirectionalInformation.PoiseAbsorption));
        }

        return value;
    }
}