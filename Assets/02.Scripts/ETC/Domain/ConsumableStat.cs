using System;
using UnityEngine;

[Serializable]
public class ConsumableStat<T> where T : struct, IConvertible
{
    [SerializeField] private T _maxValue;
    [SerializeField] private T _currentValue;
    [SerializeField] private T _regenValue;

    public T MaxValue => _maxValue;
    public T CurrentValue => _currentValue;
    public T RegenValue => _regenValue;

    public event Action ValueChanged;

    public void Init(T max, T current, T regen)
    {
        SetMaxValue(max);
        SetCurrentValue(current);
        SetRegenValue(regen);
    }

    public bool IsFull()
    {
        return EqualsDouble(_maxValue , _currentValue);
    }

    public bool IsEmpty()
    {
        return ToDouble(_currentValue) <= 0.00001;
    }


    public void Consume(T amount)
    {
        DecreaseCurrent(amount);
    }

    public void Regenerate()
    {
        if (ToDouble(_currentValue) >= ToDouble(_maxValue) ||
            ToDouble(_regenValue) <= 0)
        {
            return;
        }

        double added = ToDouble(_regenValue) * Time.deltaTime;
        IncreaseCurrent(FromDouble(added));
    }

    public void IncreaseMax(T amount)
    {
        SetMaxValue(FromDouble(ToDouble(_maxValue) + ToDouble(amount)));
    }

    public void DecreaseMax(T amount)
    {
        SetMaxValue(FromDouble(ToDouble(_maxValue) - ToDouble(amount)));
    }

    public void SetMaxValue(T amount)
    {
        double max = Math.Max(0, ToDouble(amount));
        T newMax = FromDouble(max);

        if (EqualsDouble(newMax, _maxValue))
            return;

        _maxValue = newMax;

        double current = Math.Clamp(ToDouble(_currentValue), 0, max);
        _currentValue = FromDouble(current);

        ValueChanged?.Invoke();
    }

    public void IncreaseCurrent(T amount)
    {
        SetCurrentValue(FromDouble(ToDouble(_currentValue) + ToDouble(amount)));
    }

    public void DecreaseCurrent(T amount)
    {
        SetCurrentValue(FromDouble(ToDouble(_currentValue) - ToDouble(amount)));
    }

    public void SetCurrentValue(T amount)
    {
        double value = Math.Clamp(ToDouble(amount), 0, ToDouble(_maxValue));
        T newValue = FromDouble(value);

        if (EqualsDouble(newValue, _currentValue))
            return;

        _currentValue = newValue;
        ValueChanged?.Invoke();
    }

    public void IncreaseRegen(T amount)
    {
        SetRegenValue(FromDouble(ToDouble(_regenValue) + ToDouble(amount)));
    }

    public void DecreaseRegen(T amount)
    {
        SetRegenValue(FromDouble(ToDouble(_regenValue) - ToDouble(amount)));
    }

    public void SetRegenValue(T amount)
    {
        double regen = Math.Max(0, ToDouble(amount));
        T newRegen = FromDouble(regen);

        if (EqualsDouble(newRegen, _regenValue))
            return;

        _regenValue = newRegen;
        ValueChanged?.Invoke();
    }

    private double ToDouble(T value)
    {
        return Convert.ToDouble(value);
    }

    private T FromDouble(double value)
    {
        return (T)Convert.ChangeType(value, typeof(T));
    }

    private bool EqualsDouble(T a, T b)
    {
        return Math.Abs(ToDouble(a) - ToDouble(b)) < 0.00001;
    }
}
