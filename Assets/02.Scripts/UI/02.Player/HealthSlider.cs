using DG.Tweening;
using UnityEngine;

public class HealthSlider : DoubleSliderUI
{
    [Header("Health Reference")]
    private IReadOnlyConsumable<float> Health;

    private float _lastValue;
    private Tween _delayTween;

    private void Start()
    {
        Init(Health.Max, Health.Current);
        _lastValue = Health.Current;
    }

    private void OnEnable()
    {
        if (Health == null)
            return;

        Health.Subscribe(OnHealthChanged);
        Health.Subscribe(OnMaxHealthChanged);

        Sync(Health.Current);
    }

    private void OnDisable()
    {
        if (Health == null)
            return;

        Health.Unsubscribe(OnHealthChanged);
        Health.Unsubscribe(OnMaxHealthChanged);

        _delayTween?.Kill();
        _delayTween = null;
    }

    private void OnHealthChanged(float current)
    {
        bool isIncrease = current > _lastValue;
        ApplyHealthChange(current, isIncrease);
        _lastValue = current;
    }

    private void OnMaxHealthChanged(float max)
    {
        SetMax(max);
    }

    #region Health Specific Animation

    private void ApplyHealthChange(float current, bool isIncrease)
    {
        _delayTween?.Kill();

        if (isIncrease)
        {
            BehindIncrease(current);

            _delayTween = DOVirtual.DelayedCall(1f, () =>
            {
                FrontIncrease(current);
            });
        }
        else
        {
            FrontDecrease(current);

            _delayTween = DOVirtual.DelayedCall(1f, () =>
            {
                BehindDecrease(current);
            });
        }
    }

    #endregion
}
