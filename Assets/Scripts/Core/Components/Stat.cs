using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Stat
{
    public event Action OnCurrentValueZero;
    public event Action<float, float, float> OnValueChanged;
    public event Action<Stat> OnValueDecrease;
    
    [field: SerializeField] public bool IsInfinite { get; private set; }
    [field: SerializeField] public float MaxValue { get; private set; }
    [field: SerializeField] public bool RecoveryEnabled { get; private set; }
    [field: SerializeField] public float RecoveryRate { get; private set; }
    [field: SerializeField] public float RecoveryDelay { get; private set; }

    public Coroutine RecoveryCoroutine;
    
    public float CurrentValue 
    { 
        get => IsInfinite ? int.MaxValue : currentValue;
        private set
        {
            float prevValue = currentValue;
            currentValue = Mathf.Clamp(value, 0, MaxValue);
            OnValueChanged?.Invoke(prevValue, currentValue, MaxValue);
            if (currentValue <= 0) OnCurrentValueZero?.Invoke();
        }
    }
    private float currentValue = 0;
    
    public void Init() => CurrentValue = MaxValue;
    public void SetCurrentValue(float value) => CurrentValue = value;

    public void Increase(float amount) => CurrentValue += amount;
    public void Decrease(float amount)
    {
        OnValueDecrease?.Invoke(this);
        CurrentValue -= amount;
    }
}