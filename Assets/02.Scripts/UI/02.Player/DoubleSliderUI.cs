using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class DoubleSliderUI : MonoBehaviour
{
    [Header("Slider References")]
    [SerializeField] protected Slider _backSlider;
    [SerializeField] protected Slider _frontSlider;

    [Header("Increase Tween Settings")]
    [SerializeField] protected Ease _frontUpEase = Ease.OutQuad;
    [SerializeField] protected Ease _backUpEase = Ease.OutBounce;

    [SerializeField] protected float _backUpDuration = 0.25f;
    [SerializeField] protected float _frontUpDuration = 0.15f;

    [Header("Decrease Tween Settings")]
    [SerializeField] protected Ease _frontDownEase = Ease.OutQuad;
    [SerializeField] protected Ease _backDownEase = Ease.OutBounce;

    [SerializeField] protected float _backDownDuration = 0.2f;
    [SerializeField] protected float _frontDownDuration = 0.2f;

    #region Init / Sync

    public virtual void Init(float max, float current)
    {
        SetMax(max);
        Sync(current);
    }

    public virtual void SetMax(float max)
    {
        _backSlider.maxValue = max;
        _frontSlider.maxValue = max;
    }

    // 즉시 동기화 (트윈 없음)
    public virtual void Sync(float current)
    {
        _backSlider.value = current;
        _frontSlider.value = current;
    }

    #endregion

    #region Apply Value
    /// 값 변경 연출 적용
    /// isIncrease는 외부에서 판단해서 전달
    protected void ApplyValue(float current, bool isIncrease)
    {
        if (isIncrease)
        {
            PlayIncrease(current);
        }
        else
        {
            PlayDecrease(current);
        }
    }

    #endregion

    #region Tween Plays
    protected virtual void PlayIncrease(float target)
    {
        FrontIncrease(target);
        BehindIncrease(target);
    }

    protected virtual void PlayDecrease(float target)
    {
        FrontDecrease(target);
        BehindDecrease(target);
    }

    protected virtual void FrontIncrease(float value)
    {
        _frontSlider.DOKill();

        _frontSlider
            .DOValue(value, _frontUpDuration)
            .SetEase(_frontUpEase);
    }
    protected virtual void BehindIncrease(float value)
    {
        _backSlider.DOKill();

        _backSlider
            .DOValue(value, _backUpDuration)
            .SetEase(_backUpEase);
    }
    protected virtual void FrontDecrease(float value)
    {
        _frontSlider.DOKill();

        _frontSlider
            .DOValue(value, _frontDownDuration)
            .SetEase(_frontDownEase);
    }
    protected virtual void BehindDecrease(float value)
    {
        _backSlider.DOKill();

        _backSlider
            .DOValue(value, _backDownDuration)
            .SetEase(_backDownEase);
    }

    #endregion
}
