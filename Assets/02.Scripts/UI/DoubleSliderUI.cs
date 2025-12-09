using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class DoubleSliderUI : MonoBehaviour
{
    [Header("Slider References")]
    [SerializeField] protected Slider _behindSlider;
    [SerializeField] protected Slider _frontSlider;

    [Header("Up Tween Settings")]
    [SerializeField] protected Ease _mainEaseUp = Ease.OutQuad;
    [SerializeField] protected Ease _chunkEaseUp = Ease.OutBounce;

    [SerializeField] protected float _chunkUpDuration = 0.25f;
    [SerializeField] protected float _mainUpDuration = 0.15f;

    [Header("Down Tween")]
    [SerializeField] protected Ease _downEase = Ease.Linear;
    [SerializeField] protected float _downDuration = 0.2f;

    public virtual void Init(float max, float current)
    {
        _behindSlider.maxValue = max;
        _frontSlider.maxValue = max;

        _behindSlider.value = current;
        _frontSlider.value = current;
    }

    //값이 수정될때 전방, 후방 슬라이더를 변경하는 방식
    public abstract void ChangeValue();

    protected void FrontIncrease(float frontTarget)
    {
        _frontSlider.DOKill();
        _frontSlider
            .DOValue(frontTarget, _mainUpDuration)
            .SetEase(_mainEaseUp);
    }

    protected void BehindIncrease( float behindTarget)
    {
        _behindSlider.DOKill();
        _behindSlider
            .DOValue(behindTarget, _chunkUpDuration)
            .SetEase(_chunkEaseUp);
    }

    protected void FrontDecrease(float frontTarget)
    {
        _frontSlider.DOKill();
        _frontSlider
            .DOValue(frontTarget, _downDuration)
            .SetEase(_downEase);
    }

    protected void BehindDecrease(float behindTarget)
    {
        _behindSlider.DOKill();
        _behindSlider
            .DOValue(behindTarget, _downDuration)
            .SetEase(_downEase);
    }
}
