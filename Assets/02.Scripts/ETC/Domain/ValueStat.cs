using System;
using UnityEngine;

[Serializable]
public class ValueStat<T> where T : struct, IConvertible
{
    [SerializeField] private T _value;
    public T Value => _value;

    public event Action ValueChanged;

    public void Init(T value)
    {
        _value = value;
    }

    public void Increase(float amount)
    {
        SetValue(ConvertTo<T>(ToDouble(_value) + amount));
    }

    public void Decrease(float amount)
    {
        SetValue(ConvertTo<T>(ToDouble(_value) - amount));
    }

    public void SetValue(T newValue)
    {
        // Clamp to >= 0
        double clamped = Math.Max(0, ToDouble(newValue));

        _value = ConvertTo<T>(clamped);

        Debug.Log($"[FloatValueStat] New Value: {_value}");
        ValueChanged?.Invoke();
    }

    private double ToDouble(T value)
    {
        return Convert.ToDouble(value);
    }

    private U ConvertTo<U>(double value) where U : struct, IConvertible
    {
        return (U)Convert.ChangeType(value, typeof(U));
    }
}
