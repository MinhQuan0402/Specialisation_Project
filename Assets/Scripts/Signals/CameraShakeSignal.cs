using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraShakePackage
{
    public ShakePreset shakePreset;
    public float duration;
    public float amplitude;
    public float frequency;
    public float delay;
    public CameraShakePackage(ShakePreset preset)
    {
        shakePreset = preset;
        duration = preset.duration;
        amplitude = preset.amplitude;
        frequency = preset.frequency;
        delay = preset.delay;
    }

    public CameraShakePackage(float duration, float amplitude, float frequency, float delay)
    {
        shakePreset = null;
        this.duration = duration;
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.delay = delay;
    }
}

[CustomStyle("CustomNotificationStyle")]
public class CameraShakeSignal : Marker, INotification
{
    // This is the data that will be sent with the notification
    public CameraShakePackage cameraShakePackage;

    // This ID links the notification to the receiver
    public PropertyName id => new ("CameraShakeNotificationID");
}