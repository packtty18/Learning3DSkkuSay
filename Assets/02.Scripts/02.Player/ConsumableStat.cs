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
        if(_currentValue >= _maxValue)
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
        _maxValue = amount;
        _maxValue = Mathf.Max(0, _maxValue);
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
        _currentValue = amount;
        _currentValue = Mathf.Clamp(amount, 0f, _maxValue);
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
        _regenValue = amount;
        _regenValue = Mathf.Max(0, _regenValue);
    }
}
