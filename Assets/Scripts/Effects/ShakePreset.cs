using UnityEngine;

[CreateAssetMenu(fileName = "ShakePreset_New", menuName = "Camera/Shake Preset")]
public class ShakePreset : ScriptableObject
{
    [Header("Parameters")]
    [Tooltip("How long the shake lasts in seconds.")]
    [Range(0.05f, 3f)]
    public float duration = 0.3f;

    [Tooltip("Peak displacement in world units.\n" +
             "0.05–0.15 = light hit\n" +
             "0.2–0.4   = heavy hit / explosion\n" +
             "0.5+      = boss slam / death")]
    [Range(0.01f, 1f)]
    public float amplitude = 0.2f;

    [Tooltip("Oscillations per second — controls how 'fast' the shake feels.\n" +
             "5–10  = slow, wobbly (earthquake)\n" +
             "15–25 = snappy impact (default hits)\n" +
             "30–50 = rapid vibration (gunshot, electric)")]
    [Range(1f, 60f)]
    public float frequency = 20f;

    [Tooltip("Seconds to wait before the shake starts. " +
             "Useful for syncing with a slow-mo freeze frame.")]
    [Range(0f, 10f)]
    public float delay = 0f;

    // ── Built-in preset factory methods ───────────────────────────────────────
    // If you don't want to create assets, call these from code instead:

    public static ShakePreset Light => Build(0.20f, 0.08f, 18f, 0f);
    public static ShakePreset Medium => Build(0.30f, 0.20f, 20f, 0f);
    public static ShakePreset Heavy => Build(0.45f, 0.40f, 22f, 0f);
    public static ShakePreset BossSlam => Build(0.60f, 0.55f, 14f, 0.05f);
    public static ShakePreset Death => Build(0.80f, 0.70f, 10f, 0f);

    static ShakePreset Build(float dur, float amp, float freq, float del)
    {
        var p = CreateInstance<ShakePreset>();
        p.duration = dur;
        p.amplitude = amp;
        p.frequency = freq;
        p.delay = del;
        return p;
    }
}