using UnityEngine;

/*
 * Information about the directional properties of a weapon, such as how much damage is absorbed based on the angle of attack.
 */
[System.Serializable]
public class DirectionalInformation
{
    [Range(-180.0f, 180.0f)] public float MinAngle;
    [Range(-180.0f, 180.0f)] public float MaxAngle;

    [Range(0.0f, 1.0f)] public float DamageAbsorption;
    [Range(0.0f, 1.0f)] public float KnockbackAbsorption;
    [Range(0.0f, 1.0f)] public float PoiseAbsorption;

    public bool IsAngleBetween(float angle)
    {
        if (MaxAngle > MinAngle)
        {
            return angle >= MinAngle && angle <= MaxAngle;

        }

        return (angle >= MinAngle && angle <= 180f) || (angle <= MaxAngle && angle >= -180f);
    }
}
