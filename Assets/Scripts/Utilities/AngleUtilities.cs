using UnityEngine;
using System.Collections;

public static class AngleUtilities
{
    public static float AngleFromFacingDirection(Transform receiver, Transform source, int direction)
    {
        return Vector2.SignedAngle(Vector2.right * direction, 
            source.position - receiver.position) * direction;
    }
}