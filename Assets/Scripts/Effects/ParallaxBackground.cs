using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [Tooltip("The SpriteRenderer for this background layer.")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("0 = no movement (far background). 1 = moves with camera (foreground).")]
    [Range(0f, 1f)]
    public float parallaxFactorX = 0.2f;

    [Tooltip("Vertical parallax. Usually 0 for flat platformers.")]
    [Range(0f, 1f)]
    public float parallaxFactorY = 0f;

    [Tooltip("Enable infinite horizontal tiling for this layer.")]
    public bool infiniteScrollX = true;

    [Tooltip("Enable infinite vertical tiling (useful for cave/dungeon levels).")]
    public bool infiniteScrollY = false;

    // Internal — set automatically at runtime
    [ReadOnlyInspector] public float spriteWidth;
    [ReadOnlyInspector] public float spriteHeight;
    [ReadOnlyInspector] public Vector3 startPos;
}

public class ParallaxBackground : MonoBehaviour
{
    [Header("Layers (index 0 = furthest back)")]
    public ParallaxLayer[] layers;

    [Header("Camera Reference")]
    [Tooltip("Leave empty to auto-find Camera.main at Start.")]
    public Transform cameraTransform;

    // Stores the camera position from the previous frame to compute delta
    private Vector3 _previousCamPos;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        _previousCamPos = cameraTransform.position;

        foreach (var layer in layers)
        {
            if (layer.spriteRenderer == null) continue;

            // Cache starting world position of each layer
            layer.startPos = layer.spriteRenderer.transform.position;

            // Cache the world-space width/height of the sprite for tiling
            layer.spriteWidth = layer.spriteRenderer.bounds.size.x;
            layer.spriteHeight = layer.spriteRenderer.bounds.size.y;
        }
    }

    void FixedUpdate()
    {
        Vector3 camDelta = cameraTransform.position - _previousCamPos;

        foreach (var layer in layers)
        {
            if (layer.spriteRenderer == null) continue;

            Transform t = layer.spriteRenderer.transform;

            // How much this layer should move based on its parallax factor
            float moveX = camDelta.x * layer.parallaxFactorX;
            float moveY = camDelta.y * layer.parallaxFactorY;

            // Apply the parallax offset
            t.position += new Vector3(moveX, moveY, 0f);

            // When the camera travels far enough that the sprite edge would
            // become visible, we jump the layer by one sprite-width so it tiles
            // seamlessly without needing multiple copies in the scene.

            if (layer.infiniteScrollX)
            {
                float distX = cameraTransform.position.x - t.position.x;
                if (Mathf.Abs(distX) >= layer.spriteWidth * 0.5f)
                {
                    // Snap the layer to stay centred under the camera
                    float offset = distX > 0
                        ? layer.spriteWidth
                        : -layer.spriteWidth;

                    t.position += new Vector3(offset, 0f, 0f);
                }
            }

            if (layer.infiniteScrollY)
            {
                float distY = cameraTransform.position.y - t.position.y;
                if (Mathf.Abs(distY) >= layer.spriteHeight * 0.5f)
                {
                    float offset = distY > 0
                        ? layer.spriteHeight
                        : -layer.spriteHeight;

                    t.position += new Vector3(0f, offset, 0f);
                }
            }
        }

        // Store this frame's camera position for next frame's delta calculation
        _previousCamPos = cameraTransform.position;
    }
}