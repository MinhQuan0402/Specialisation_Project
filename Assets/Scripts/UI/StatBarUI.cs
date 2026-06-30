using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class StatBarUI : MonoBehaviour
{
    public enum StatBarType
    {
        HEALTH,
        STAMINA,
        POISE
    }

    [field: SerializeField] public StatBarType barType;
    [SerializeField] private Slider fillSlider;
    [SerializeField] private float smoothSpeed = 5f;

    private float targetFill;

    public void SetInitValue(float initValue, float maxValue)
    {
        fillSlider.maxValue = maxValue;
        fillSlider.value = initValue;
        targetFill = initValue;
    }

    public void SetTarget(float current, float max)
    {
        targetFill = current;
        fillSlider.maxValue = max;
    }

    private void Update()
    {
        fillSlider.value = Mathf.Lerp(
            fillSlider.value, targetFill, Time.deltaTime * smoothSpeed);
    }
}