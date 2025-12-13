using UnityEngine;

public class StaminaSliderUI : DoubleSliderUI
{
    [Header("Player Reference")]
    [SerializeField] private PlayerStat _stat;

    [Header("Chunk Settings")]
    [SerializeField] private float _chunkValue = 20f;

    private float _cachedChunkIndex;
    private float _lastValue;

    private ConsumableStat<float> Stamina => _stat.Stemina;

    private void Start()
    {
        Init(Stamina.Max, Stamina.Current);
        SyncInitialState();
    }

    private void OnEnable()
    {
        if (Stamina == null)
            return;

        Stamina.OnCurrentChanged += OnStaminaChanged;
        Stamina.OnMaxChanged += OnMaxStaminaChanged;

        SyncInitialState();
    }

    private void OnDisable()
    {
        if (Stamina == null)
            return;

        Stamina.OnCurrentChanged -= OnStaminaChanged;
        Stamina.OnMaxChanged -= OnMaxStaminaChanged;
    }

    private void SyncInitialState()
    {
        float current = Stamina.Current;

        _cachedChunkIndex = Mathf.Floor(current / _chunkValue);
        _lastValue = current;

        OnStaminaChanged(current);
    }

    private void OnStaminaChanged(float current)
    {
        bool isIncreasing = current > _lastValue;

        float currentChunkStart = _cachedChunkIndex * _chunkValue;
        float nextChunkStart = (_cachedChunkIndex + 1) * _chunkValue;

        if (!isIncreasing)
        {
            if (current < currentChunkStart)
            {
                _cachedChunkIndex = Mathf.Max(0f, _cachedChunkIndex - 1);
            }

            FrontDecrease(_cachedChunkIndex * _chunkValue);
            BehindDecrease(current);
        }
        else
        {
            if (current >= nextChunkStart)
            {
                _cachedChunkIndex++;
            }

            FrontIncrease(_cachedChunkIndex * _chunkValue);
            BehindIncrease(current);
        }

        _lastValue = current;
    }

    private void OnMaxStaminaChanged(float max)
    {
        SetMax(max);
    }
}
