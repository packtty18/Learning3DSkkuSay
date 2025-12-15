using System;
using UnityEngine;

[Serializable]
public abstract class StatBase<T> where T : struct, IConvertible
{
    public event Action<T> OnValueChanged;

    protected void Notify(T value)
    {
        OnValueChanged?.Invoke(value);
    }

    //더블형식으로 바꾼다
    protected double ToDouble(T value)
    {
        return Convert.ToDouble(value);
    }

    //T형식으로 바꾼다
    protected T FromDouble(double value)
    {
        return (T)Convert.ChangeType(value, typeof(T));
    }

    //두개의 값이 같은지 체크한다.
    protected bool EqualsValue(T a, T b, double epsilon = 0.00001)
    {
        return Math.Abs(ToDouble(a) - ToDouble(b)) < epsilon;
    }

    //음수의 경우 0으로
    protected T ClampMinZero(T value)
    {
        return FromDouble(Math.Max(0, ToDouble(value)));
    }
}
