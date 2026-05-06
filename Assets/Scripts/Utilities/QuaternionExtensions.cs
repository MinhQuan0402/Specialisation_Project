using UnityEngine;

namespace Utilities
{
    public static class QuaternionExtensions
    {
        public static Quaternion Vector2ToRotation(Vector2 direction)
        {
            // Calculate the angle between the vector and the positive x-axis
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Create a quaternion rotation around the positive z-axis
            var rotation = Quaternion.Euler(0f, 0f, angle);

            return rotation;
        }
    }
}