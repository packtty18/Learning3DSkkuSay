using System;
using UnityEngine;

[Serializable]
public class ConsumableStat<T> : StatBase<T> where T : struct, IConvertible
{
    [SerializeField] private T _max;
    [SerializeField] private T _current;

    [SerializeField] private float _regen;

    private float _regenAccumulator;

    public T Max => _max;
    public T Current => _current;
    public float RegenValue => _regen;

    public event Action<T> OnMaxChanged;
    public event Action<T> OnCurrentChanged;
    public event Action<float> OnRegenChanged;

    public void Init(T max, T current = default, float regen = 0f)
    {
        SetMax(max);

        if (ToDouble(current) <= 0.00001)
            SetCurrent(_max);
        else
            SetCurrent(current);

        SetRegen(regen);
        _regenAccumulator = 0f;
    }

    #region State Check
    public bool IsFull()
    {
        return EqualsValue(_current, _max);
    }

    public bool IsEmpty()
    {
        return ToDouble(_current) <= 0.00001;
    }

    public float GetRatio()
    {
        double max = ToDouble(_max);

        if (max <= 0.00001)
            return 0f;

        return (float)(ToDouble(_current) / max);
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
        double clamped = Math.Clamp(
            ToDouble(value),
            0,
            ToDouble(_max));

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
    public void SetRegen(float value)
    {
        float clamped = Mathf.Max(0f, value);

        if (Mathf.Approximately(_regen, clamped))
            return;

        _regen = clamped;
        OnRegenChanged?.Invoke(_regen);
    }

    public void Regenerate(float deltaTime)
    {
        if (IsFull() || _regen <= 0f)
            return;

        _regenAccumulator += _regen * deltaTime;

        if (typeof(T) == typeof(float))
        {
            SetCurrent(FromDouble(ToDouble(_current) + _regen * deltaTime));
            return;
        }

        if (_regenAccumulator < 1f)
            return;

        int add = Mathf.FloorToInt(_regenAccumulator);
        _regenAccumulator -= add;

        SetCurrent(FromDouble(ToDouble(_current) + add));
    }
    #endregion
}
