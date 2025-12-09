using Unity.VisualScripting;
using UnityEngine;

public class StaminaSliderUI : DoubleSliderUI
{
    [Header("Player Reference")]
    [SerializeField] private PlayerStat _stat;

    [Header("Chunk Settings")]
    [SerializeField] private float _chunkValue = 20f;

    private float _cachedChunkIndex = 0f;
    private float _lastValue = 0f;

    private ConsumableStat Stamina => _stat.Stemina;
    

    private void Start()
    {
        _cachedChunkIndex = Mathf.Floor(Stamina.CurrentValue / _chunkValue);

        Init(Stamina.MaxValue, Stamina.CurrentValue);
    }

    private void OnEnable()
    {
        _stat.Stemina.ValueChanged +=  ChangeValue;
    }

    private void OnDisable()
    {
        _stat.Stemina.ValueChanged -= ChangeValue;
    }

    public override void ChangeValue()
    {
        float value = Stamina.CurrentValue;
        bool isIncreasing = value > _lastValue;
        _lastValue = value;

        float currentChunkStart = _cachedChunkIndex * _chunkValue;
        float nextChunkStart = (_cachedChunkIndex + 1) * _chunkValue;

        if (!isIncreasing)
        {
            if (value < currentChunkStart)
            {
                _cachedChunkIndex = Mathf.Max(0f, _cachedChunkIndex - 1);
            }
            FrontDecrease(_cachedChunkIndex * _chunkValue);
            BehindDecrease(value);
            return;
        }

        bool chunkPassed = value >= nextChunkStart;

        if (chunkPassed)
        {
            _cachedChunkIndex++;
        }

        FrontIncrease(_cachedChunkIndex * _chunkValue );
        BehindIncrease(value);
    }
}
