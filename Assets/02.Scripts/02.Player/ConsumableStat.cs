using System;
using UnityEngine;

[Serializable]
public class ConsumableStat
{
    [SerializeField] private float _maxValue;
    [SerializeField] private float _currentValue;
    [SerializeField] private float _regenValue;

    public float MaxValue => _maxValue;
    public float CurrentValue => _currentValue;
    public float RegenValue => _regenValue;

    public event Action ValueChanged;

    public void Init(float max, float current, float regen)
    {
        SetMaxValue(max);
        SetCurrentValue(current);
        SetRegenValue(regen);
    }

    public void Consume(float amount)
    {
        DecreaseCurrent(amount);
    }

    public void Regenerate()
    {
        if (_currentValue >= _maxValue || _regenValue <= 0)
        {
            return;
        }
        IncreaseCurrent(_regenValue * Time.deltaTime);
    }

    public void IncreaseMax(float amount) 
    { 
        SetMaxValue(_maxValue + amount); 
    }
    public void DecreaseMax(float amount)
    {
        SetMaxValue(_maxValue - amount);
    }

    public void SetMaxValue(float amount)
    {
        float newMax = Mathf.Max(0, amount);

        if (Mathf.Approximately(newMax, _maxValue))
        {
            return;
        }
        _maxValue = newMax;

        _currentValue = Mathf.Clamp(_currentValue, 0, _maxValue);
        ValueChanged?.Invoke();
    }

    public void IncreaseCurrent(float amount)
    {
        SetCurrentValue(_currentValue + amount);
    }
    public void DecreaseCurrent(float amount)
    {
        SetCurrentValue(_currentValue - amount);
    }

    public void SetCurrentValue(float amount)
    {
        float newValue = Mathf.Clamp(amount, 0, _maxValue);

        if (Mathf.Approximately(newValue, _currentValue))
        {
            return;
        }

        _currentValue = newValue;
        ValueChanged?.Invoke();
    }

    public void IncreaseRegen(float amount)
    {
        SetRegenValue(_regenValue + amount);
    }
    public void DecreaseRegen(float amount)
    {
        SetRegenValue(_regenValue - amount);
    }

    public void SetRegenValue(float amount)
    {
        float newRegen = Mathf.Max(0, amount);
        if (Mathf.Approximately(newRegen, _regenValue))
            return;

        _regenValue = newRegen;
        ValueChanged?.Invoke();
    }
}
