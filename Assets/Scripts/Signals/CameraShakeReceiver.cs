using UnityEngine;
using UnityEngine.Playables;

public class CameraShakeReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        // Check if the received notification matches your custom type
        if (notification is CameraShakeSignal cameraShakeSignal)
        {
            // Access your custom data variables directly
            TriggerShakeAction(cameraShakeSignal.cameraShakePackage);
        }
    }

    private void TriggerShakeAction(CameraShakePackage shakePackage)
    {
        if (shakePackage.shakePreset != null) CameraShakeCinemachine.Instance.ShakeFromPreset(shakePackage.shakePreset);
        else CameraShakeCinemachine.Shake(shakePackage.duration, shakePackage.amplitude, shakePackage.frequency, shakePackage.delay);
    }
}
