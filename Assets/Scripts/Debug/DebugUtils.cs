using UnityEngine;

public static class DebugUtils
{
    public static void DrawBox2D(Vector2 center, Vector2 size, Color color, float duration = 0f)
    {
        Vector2 half = size * 0.5f;

        // Calculate the 4 corners of the 2D box
        Vector3 topLeft = new Vector3(center.x - half.x, center.y + half.y, 0f);
        Vector3 topRight = new Vector3(center.x + half.x, center.y + half.y, 0f);
        Vector3 bottomLeft = new Vector3(center.x - half.x, center.y - half.y, 0f);
        Vector3 bottomRight = new Vector3(center.x + half.x, center.y - half.y, 0f);

        // Draw the 4 bounding lines
        Debug.DrawLine(topLeft, topRight, color, duration);      // Top edge
        Debug.DrawLine(topRight, bottomRight, color, duration);  // Right edge
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);// Bottom edge
        Debug.DrawLine(bottomLeft, topLeft, color, duration);    // Left edge
    }
}