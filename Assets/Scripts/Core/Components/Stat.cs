using System;
using UnityEngine;

[Serializable]
public class Stat
{
    public event Action OnCurrentValueZero;
    
    [field: SerializeField] public bool IsInfinite { get; private set; }
    [field: SerializeField] public float MaxValue { get; private set; }
    
    public float CurrentValue 
    { 
        get => IsInfinite ? int.MaxValue : currentValue;
        private set
        {
            currentValue = Mathf.Clamp(value, 0, MaxValue);
            if (currentValue <= 0) OnCurrentValueZero?.Invoke();
        }
    }
    private float currentValue = 0;
    
    public void Init() => CurrentValue = MaxValue;
    
    public void Increase(float amount) => CurrentValue += amount;
    public void Decrease(float amount) => CurrentValue -= amount;
}