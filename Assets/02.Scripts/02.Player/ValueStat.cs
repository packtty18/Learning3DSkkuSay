using System;
using UnityEngine;

[Serializable]
public class ValueStat
{
    [SerializeField] private float _value;
    public float Value => _value;

    public void Init(float value)
    {
        _value = value;
    }

    public void Increase(float amount)
    {
        SetValue(_value + amount);
    }

    public void Decrease(float amount)
    {
        SetValue(_value - amount);
    }
    public void SetValue(float amount)
    {
        _value = amount;
        Mathf.Max(0, _value);
    }
}

