using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DoubleSliderUI : MonoBehaviour
{
    [Header("Slider References")]
    [SerializeField] protected Slider _chunkSlider;
    [SerializeField] protected Slider _mainSlider;
    [SerializeField] protected float _chunkValue = 20f;

    [Header("Up Tween Settings")]
    [SerializeField] protected Ease _mainEaseUp = Ease.OutQuad;
    [SerializeField] protected Ease _chunkEaseUp = Ease.OutBounce;

    [SerializeField] protected float _chunkUpDuration = 0.25f;
    [SerializeField] protected float _mainUpDuration = 0.15f;

    [Header("Down Tween")]
    [SerializeField] protected Ease downEase = Ease.Linear;
    [SerializeField] protected float downDuration = 0.2f;

    protected float cachedChunkIndex = 0f;
    protected float lastValue = 0f;

    public virtual void Init(float max, float current)
    {
        _chunkSlider.maxValue = max;
        _mainSlider.maxValue = max;

        cachedChunkIndex = Mathf.Floor(current / _chunkValue);

        _chunkSlider.value = cachedChunkIndex * _chunkValue;
        _mainSlider.value = current;

        lastValue = current;
    }

    public virtual void SetValue(float value)
    {
        float currentChunkStart = cachedChunkIndex * _chunkValue;
        float nextChunkStart = (cachedChunkIndex + 1) * _chunkValue;

        bool isIncreasing = value > lastValue;
        lastValue = value;

        if (!isIncreasing)
        {
            if (value < currentChunkStart)
            {
                cachedChunkIndex = Mathf.Max(0, cachedChunkIndex - 1);
            }

            float mainTarget = cachedChunkIndex * _chunkValue;
            _chunkSlider.DOKill();
            _chunkSlider.DOValue(mainTarget, downDuration).SetEase(downEase);
            _mainSlider.DOKill();
            _mainSlider.DOValue(value, downDuration).SetEase(downEase);

            return;
        }

        _mainSlider
            .DOValue(value, _mainUpDuration)
            .SetEase(_mainEaseUp);

        if (value >= nextChunkStart)
        {
            cachedChunkIndex++;

            float mainTarget = cachedChunkIndex * _chunkValue;

            _chunkSlider
                .DOValue(mainTarget, _chunkUpDuration)
                .SetEase(_chunkEaseUp);
        }
    }

}
