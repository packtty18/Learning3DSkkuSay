using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine;

public class HealthSlider : DoubleSliderUI
{
    [Title("Player Reference")]
    [Required, SerializeField]
    private PlayerStat _stat;

    private IReadOnlyConsumable<float> Health => _stat.GetConsumable(EConsumableFloat.Health);

    private float _lastValue;
    private Tween _delayTween;

    private void OnEnable()
    {
        if (Health == null)
        {
            _stat.OnStatInitEnd.Subscribe(UIEnable);
            return;

        }
        UIEnable();
    }

    private void UIEnable()
    {
        Health.Subscribe(OnHealthChanged);
        Health.Subscribe(OnMaxHealthChanged);

        Init(Health.Max, Health.Current);
        _lastValue = Health.Current;
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
