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

    [Tooltip("How long (seconds) the layer takes to reach its target position.\n" +
             "0 = instant (old snappy behaviour).\n" +
             "0.05–0.15 = subtle smoothing (recommended for near layers).\n" +
             "0.15–0.4  = dreamy float (great for far sky/fog layers).")]
    [Range(0f, 0.5f)]
    public float smoothTime = 0.1f;

    [Tooltip("Cap on how fast this layer can move (units/sec). 0 = uncapped.")]
    public float maxSpeed = 50f;


    [Tooltip("Enable infinite horizontal tiling for this layer.")]
    public bool infiniteScrollX = true;

    [Tooltip("Enable infinite vertical tiling (useful for cave/dungeon levels).")]
    public bool infiniteScrollY = false;

    // Internal — set automatically at runtime
    [ReadOnlyInspector] public float spriteWidth;
    [ReadOnlyInspector] public float spriteHeight;
    [ReadOnlyInspector] public Vector3 targetPos;
    [ReadOnlyInspector] public Vector3 velocity;    // SmoothDamp velocity accumulator
}

public class ParallaxBackground : MonoBehaviour
{
    [Header("Layers (index 0 = furthest back)")]
    public ParallaxLayer[] layers;

    [Header("Camera Reference")]
    [Tooltip("Leave empty to auto-find Camera.main at Start.")]
    public Transform cameraTransform;

    [Header("Global Smoothing Override")]
    [Tooltip("When ON, all layers share this smoothTime instead of their own.")]
    public bool useGlobalSmooth = false;
    [Range(0f, 0.5f)]
    public float globalSmoothTime = 0.12f;

    public bool useLerpInY = true;

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

            Transform t = layer.spriteRenderer.transform;

            // Cache the world-space width/height of the sprite for tiling
            layer.spriteWidth = layer.spriteRenderer.bounds.size.x;
            layer.spriteHeight = layer.spriteRenderer.bounds.size.y;

            // Both the visual and the target start at the same place
            layer.targetPos = t.position;
            layer.velocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        Vector3 camDelta = cameraTransform.position - _previousCamPos;

        foreach (var layer in layers)
        {
            if (layer.spriteRenderer == null) continue;

            Transform t = layer.spriteRenderer.transform;

            // ── 1. Advance the logical target by the parallax-scaled delta ───
            layer.targetPos.x += camDelta.x * layer.parallaxFactorX;
            layer.targetPos.y += camDelta.y * layer.parallaxFactorY;

            // ── 2. Handle infinite tiling on the TARGET, not the visual ──────
            if (layer.infiniteScrollX)
            {
                float distX = cameraTransform.position.x - layer.targetPos.x;
                if (Mathf.Abs(distX) >= layer.spriteWidth * 0.5f)
                {
                    float jump = distX > 0 ? layer.spriteWidth : -layer.spriteWidth;
                    layer.targetPos.x += jump;

                    // Snap the visual too — no smooth across a tile seam
                    Vector3 snap = t.position;
                    snap.x += jump;
                    t.position = snap;
                }
            }

            if (layer.infiniteScrollY)
            {
                float distY = cameraTransform.position.y - layer.targetPos.y;
                if (Mathf.Abs(distY) >= layer.spriteHeight * 0.5f)
                {
                    float jump = distY > 0 ? layer.spriteHeight : -layer.spriteHeight;
                    layer.targetPos.y += jump;

                    Vector3 snap = t.position;
                    snap.y += jump;
                    t.position = snap;
                }
            }

            // ── 3. Smoothly move the visual toward the target ────────────────
            float smooth = useGlobalSmooth ? globalSmoothTime : layer.smoothTime;
            float speed = layer.maxSpeed > 0f ? layer.maxSpeed : Mathf.Infinity;

            if (smooth <= 0f)
            {
                // Zero smooth time = old instant behaviour (backwards compatible)
                t.position = layer.targetPos;
            }
            else
            {
                t.position = Vector3.SmoothDamp(
                    t.position,
                    layer.targetPos,
                    ref layer.velocity,
                    smooth,
                    speed,
                    Time.deltaTime
                );

                if (!useLerpInY) t.position = new Vector3(t.position.x, layer.targetPos.y, t.position.z);
            }
        }

        // Store this frame's camera position for next frame's delta calculation
        _previousCamPos = cameraTransform.position;
    }

    public void SnapAllToCamera()
    {
        _previousCamPos = cameraTransform.position;
        foreach (var layer in layers)
        {
            if (layer.spriteRenderer == null) continue;
            layer.velocity = Vector3.zero;
            layer.targetPos = layer.spriteRenderer.transform.position;
        }
    }

    // Adjust a single layer's speed at runtime
    public void SetLayerSpeedX(int index, float newFactor)
    {
        if (index < 0 || index >= layers.Length) return;
        layers[index].parallaxFactorX = Mathf.Clamp01(newFactor);
    }
}