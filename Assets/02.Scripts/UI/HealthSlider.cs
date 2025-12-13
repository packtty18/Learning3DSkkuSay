using DG.Tweening;
using UnityEngine;

public class HealthSlider : DoubleSliderUI
{
    [Header("Health Reference")]
    [SerializeField] private PlayerStat _stat;

    private ConsumableStat<float> Health => _stat.Health;

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

        Health.OnCurrentChanged += OnHealthChanged;
        Health.OnMaxChanged += OnMaxHealthChanged;

        Sync(Health.Current);
    }

    private void OnDisable()
    {
        if (Health == null)
            return;

        Health.OnCurrentChanged -= OnHealthChanged;
        Health.OnMaxChanged -= OnMaxHealthChanged;

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
            // 증가: 뒤 → 앞 (딜레이)
            BehindIncrease(current);

            _delayTween = DOVirtual.DelayedCall(1f, () =>
            {
                FrontIncrease(current);
            });
        }
        else
        {
            // 감소: 앞 → 뒤 (딜레이)
            FrontDecrease(current);

            _delayTween = DOVirtual.DelayedCall(1f, () =>
            {
                BehindDecrease(current);
            });
        }
    }

    #endregion
}
