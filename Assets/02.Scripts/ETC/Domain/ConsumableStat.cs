using System;
using UnityEngine;

[Serializable]
public class ConsumableStat<T> : StatBase<T> where T : struct, IConvertible
{
    [SerializeField] private T _max;
    [SerializeField] private T _current;
    [SerializeField] private T _regen;

    public T Max => _max;
    public T Current => _current;
    public T RegenValue => _regen;

    public event Action<T> OnMaxChanged;
    public event Action<T> OnCurrentChanged;
    public event Action<T> OnRegenChanged;

    public void Init(T max, T current = default, T regen = default)
    {
        SetMax(max);
        if (ToDouble(current) <= 0.00001)
        {
            SetCurrent(_max);
        }
        else
        {
            SetCurrent(current);
        }
        if (ToDouble(regen) <= 0.00001)
        {
            SetRegen(FromDouble(0));
        }
        else
        {
            SetRegen(regen);
        }
    }


    #region 상태 체크
    public bool IsFull()
    {
        return EqualsValue(_current, _max);
    }

    public bool IsEmpty()
    {
        return ToDouble(_current) <= 0.00001;
    }
    #endregion

    #region Max
    public void SetMax(T value)
    {
        T clamped = ClampMinZero(value);

        if (EqualsValue(_max, clamped))
            return;

        _max = clamped;

        // current 자동 보정
        SetCurrent(_current);

        OnMaxChanged?.Invoke(_max);
    }
    #endregion

    #region Current
    public void SetCurrent(T value)
    {
        double clamped =
            Math.Clamp(ToDouble(value), 0, ToDouble(_max));

        T newValue = FromDouble(clamped);

        if (EqualsValue(_current, newValue))
            return;

        _current = newValue;
        OnCurrentChanged?.Invoke(_current);
    }

    public void Consume(T amount)
    {
        SetCurrent(FromDouble(ToDouble(_current) - ToDouble(amount)));
    }
    #endregion

    #region Regen
    public void SetRegen(T value)
    {
        T clamped = ClampMinZero(value);

        if (EqualsValue(_regen, clamped))
            return;

        _regen = clamped;
        OnRegenChanged?.Invoke(_regen);
    }

    public void Regenerate(float deltaTime)
    {
        if (IsFull() || ToDouble(_regen) <= 0)
            return;

        double add = ToDouble(_regen) * deltaTime;
        SetCurrent(FromDouble(ToDouble(_current) + add));
    }
    #endregion
}
