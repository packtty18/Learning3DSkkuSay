using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class DoubleSliderUI : MonoBehaviour
{
    [Header("Slider References")]
    [SerializeField] protected Slider _backSlider;
    [SerializeField] protected Slider _frontSlider;

    [Header("Up Tween Settings")]
    [SerializeField] protected Ease _frontSliderUpEase = Ease.OutQuad;
    [SerializeField] protected Ease _backSliderUpEase = Ease.OutBounce;

    [SerializeField] protected float _backSliderUPDuration = 0.25f;
    [SerializeField] protected float _frontSliderUPDuration = 0.15f;

    [Header("Down Tween")]
    [SerializeField] protected Ease _frontSliderDownEase = Ease.OutQuad;
    [SerializeField] protected Ease _backSliderDownEase = Ease.OutBounce;

    [SerializeField] protected float _backSliderDownEDuration = 0.2f;
    [SerializeField] protected float _frontSliderDownEDuration = 0.2f;

    public virtual void Init(float max, float current)
    {
        _backSlider.maxValue = max;
        _frontSlider.maxValue = max;

        _backSlider.value = current;
        _frontSlider.value = current;
    }

    //값이 수정될때 전방, 후방 슬라이더를 변경하는 방식
    public abstract void ChangeValue();

    protected virtual void FrontIncrease(float frontTarget)
    {
        _frontSlider.DOKill();
        _frontSlider
            .DOValue(frontTarget, _frontSliderUPDuration)
            .SetEase(_frontSliderUpEase);
    }

    protected virtual void BehindIncrease( float behindTarget)
    {
        _backSlider.DOKill();
        _backSlider
            .DOValue(behindTarget, _backSliderUPDuration)
            .SetEase(_backSliderUpEase);
    }

    protected virtual void FrontDecrease(float frontTarget)
    {
        _frontSlider.DOKill();
        _frontSlider
            .DOValue(frontTarget, _frontSliderDownEDuration)
            .SetEase(_frontSliderDownEase);
    }

    protected virtual void BehindDecrease(float behindTarget)
    {
        _backSlider.DOKill();
        _backSlider
            .DOValue(behindTarget, _backSliderDownEDuration)
            .SetEase(_backSliderDownEase);
    }
}
