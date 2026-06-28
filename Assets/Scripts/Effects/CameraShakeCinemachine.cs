using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using System.Linq;

public class CameraShakeCinemachine : SingletonTemplate<CameraShakeCinemachine>
{
    // ── Virtual camera reference ──────────────────────────────────────────────
    [Header("Cinemachine")]
    [Tooltip("The CinemachineVirtualCamera that has a Noise component.\n" +
             "Add the Noise extension on the Virtual Camera in the Inspector:\n" +
             "Noise → select a profile (e.g. Handheld_tele_mild or 6D Shake).")]
    [SerializeField] private CinemachineCamera virtualCamera;

    [Tooltip("Add multiple Virtual Cameras here if you want every camera\n" +
             "to shake simultaneously (e.g. boss cam + main cam).")]
    [SerializeField] private CinemachineCamera[] extraVirtualCameras;

    [SerializeField] private NoiseSettings defaultNoiseSettings;

    // ── Preset slots (drag ShakePreset assets in) ─────────────────────────────
    [Header("Named Presets  (used by ShakeLight / ShakeMedium / ShakeHeavy)")]
    [SerializeField] ShakePreset lightPreset;
    [SerializeField] ShakePreset mediumPreset;
    [SerializeField] ShakePreset heavyPreset;

    // ── Falloff ───────────────────────────────────────────────────────────────
    [Header("Falloff Curve")]
    [Tooltip("How amplitude decays over the shake duration.\n" +
             "X = normalised time (0→1),  Y = amplitude multiplier.\n" +
             "Default: starts at full strength, eases to 0.")]
    [SerializeField] private AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Limits")]
    [Range(1, 20)]
    [SerializeField] private int maxConcurrentShakes = 8;

    [SerializeField] private bool testNoise = false;

    // ── Internal noise component refs ─────────────────────────────────────────
    private CinemachineBasicMultiChannelPerlin _mainNoise;
    private readonly List<CinemachineBasicMultiChannelPerlin> _extraNoise = new();

    // ── Active shake list ─────────────────────────────────────────────────────
    private readonly List<ShakeInstance> _activeShakes = new List<ShakeInstance>();

    // ─────────────────────────────────────────────────────────────────────────
    protected override void Awake()
    {
        base.Awake();

        CacheNoiseComponents();
    }

    void CacheNoiseComponents()
    {
        if (virtualCamera == null) 
            virtualCamera = GameObject.Find("PlayerCam").GetComponent<CinemachineCamera>();

        // Main virtual camera noise
        if (virtualCamera != null)
        {
            _mainNoise = virtualCamera
                .GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;

            if (_mainNoise == null)
                _mainNoise = virtualCamera.AddComponent<CinemachineBasicMultiChannelPerlin>();

            _mainNoise.NoiseProfile = defaultNoiseSettings;
        }

        if (extraVirtualCameras.Length == 0)
        {
            var allVirtualCam = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include).ToList();
            allVirtualCam.Remove(virtualCamera);
            extraVirtualCameras = allVirtualCam.ToArray();
        }


        // Extra virtual cameras
        if (extraVirtualCameras != null)
        {
            foreach (var vc in extraVirtualCameras)
            {
                if (vc == null || vc == virtualCamera) continue;
                var n = vc.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
                if (n == null) n = vc.AddComponent<CinemachineBasicMultiChannelPerlin>();
                n.NoiseProfile = defaultNoiseSettings;
                _extraNoise.Add(n);
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Update — accumulate all active shakes and push result into Cinemachine
    // ─────────────────────────────────────────────────────────────────────────
    void Update()
    {
        if (_activeShakes.Count == 0 && !testNoise)
        {
            SetNoise(0f, 0f);
            return;
        }

        float totalAmplitude = 0f;
        float highestFreq = 0f;

        for (int i = _activeShakes.Count - 1; i >= 0; i--)
        {
            ShakeInstance s = _activeShakes[i];
            s.elapsed += Time.deltaTime;

            if (s.elapsed >= s.duration)
            {
                _activeShakes.RemoveAt(i);
                continue;
            }

            float falloff = falloffCurve.Evaluate(s.elapsed / s.duration);
            totalAmplitude += s.amplitude * falloff;
            highestFreq = Mathf.Max(highestFreq, s.frequency * falloff);
        }

        // Push final values into every registered noise component
        SetNoise(totalAmplitude, highestFreq);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  PUBLIC STATIC API  —  identical interface to CameraShake.cs
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Trigger a shake with explicit parameters.
    /// Call from any script: CameraShakeCinemachine.Shake(duration, amplitude, frequency, delay);
    /// </summary>
    public static void Shake(float duration, float amplitude,
                             float frequency, float delay = 0f)
    {
        if (Instance == null)
        {
            Debug.LogWarning("[CameraShakeCinemachine] No instance found in scene.");
            return;
        }
        Instance.AddShake(duration, amplitude, frequency, delay);
    }

    /// <summary>Trigger a shake from a ShakePreset asset (static version).</summary>
    public static void Shake(ShakePreset preset)
    {
        if (preset == null) return;
        Shake(preset.duration, preset.amplitude, preset.frequency, preset.delay);
    }

    /// <summary>Stop all active shakes and reset noise to zero.</summary>
    public static void StopAll()
    {
        if (Instance == null) return;
        Instance._activeShakes.Clear();
        Instance.SetNoise(0f, 0f);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  PUBLIC INSTANCE API  —  for Unity Events (drag this component into slot)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Unity Event — pass a ShakePreset asset.
    /// Inspector: drag this GameObject → ShakeFromPreset → drag preset asset.
    /// </summary>
    public void ShakeFromPreset(ShakePreset preset)
    {
        if (preset == null) return;
        AddShake(preset.duration, preset.amplitude, preset.frequency, preset.delay);
    }

    /// <summary>Light shake — small hit, footstep landing, block.</summary>
    public void ShakeLight()
    {
        if (lightPreset != null) ShakeFromPreset(lightPreset);
        else AddShake(0.20f, 0.08f, 1.5f, 0f);
    }

    /// <summary>Medium shake — sword hit, projectile impact, fall damage.</summary>
    public void ShakeMedium()
    {
        if (mediumPreset != null) ShakeFromPreset(mediumPreset);
        else AddShake(0.30f, 0.20f, 2.0f, 0f);
    }

    /// <summary>Heavy shake — explosion, boss attack, player death.</summary>
    public void ShakeHeavy()
    {
        if (heavyPreset != null) ShakeFromPreset(heavyPreset);
        else AddShake(0.50f, 0.45f, 2.5f, 0f);
    }

    public void StopShake() => StopAll();

    // ─────────────────────────────────────────────────────────────────────────
    //  INTERNAL
    // ─────────────────────────────────────────────────────────────────────────

    void AddShake(float duration, float amplitude, float frequency, float delay)
    {
        if (_activeShakes.Count >= maxConcurrentShakes) return;

        if (delay > 0f)
            StartCoroutine(DelayedAdd(duration, amplitude, frequency, delay));
        else
            _activeShakes.Add(new ShakeInstance
            {
                duration = duration,
                amplitude = amplitude,
                frequency = frequency,
                elapsed = 0f
            });
    }

    IEnumerator DelayedAdd(float duration, float amplitude, float frequency, float delay)
    {
        yield return new WaitForSeconds(delay);
        AddShake(duration, amplitude, frequency, delay: 0f);
    }

    void SetNoise(float amplitude, float frequency)
    {
        if (_mainNoise != null)
        {
            _mainNoise.AmplitudeGain = amplitude;
            _mainNoise.FrequencyGain = frequency;
        }

        foreach (var n in _extraNoise)
        {
            if (n == null) continue;
            n.AmplitudeGain = amplitude;
            n.FrequencyGain = frequency;
        }
    }

    private class ShakeInstance
    {
        public float duration;
        public float amplitude;
        public float frequency;
        public float elapsed;
    }
}