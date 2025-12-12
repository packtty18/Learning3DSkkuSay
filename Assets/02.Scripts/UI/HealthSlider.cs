using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class HealthSlider : DoubleSliderUI
{
    [Header("Health Reference")]

    [SerializeField] private PlayerStat _stat ;
    [SerializeField] private ConsumableStat<float> _health => _stat.Health;

    private float _lastValue;
    private Tween _delayTween;

    private void Start()
    {
        Init(_health.MaxValue, _health.CurrentValue);
        ChangeValue();
    }

    public override void Init(float max, float current)
    {
        base.Init(max, current);
        _lastValue = _health.CurrentValue;
    }

    private void OnEnable()
    {
        _health.ValueChanged += ChangeValue;
    }

    private void OnDisable()
    {
        _health.ValueChanged -= ChangeValue;
    }

    public override void ChangeValue()
    {
        float current = _health.CurrentValue;

        bool isIncrease = current > _lastValue;

        if (isIncrease)
        {
            BehindIncrease(current);

            _delayTween?.Kill();
            _delayTween = DOVirtual.DelayedCall(1f, () =>
            {
                FrontIncrease(current);
            });

        }
        else
        {
            FrontDecrease(current);
            _delayTween?.Kill();
            _delayTween = DOVirtual.DelayedCall(1f, () =>
            {
                BehindDecrease(current);
            });

        }

        _lastValue = current;
    }
}
