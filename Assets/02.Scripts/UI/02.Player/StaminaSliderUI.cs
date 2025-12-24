using UnityEngine;
using Sirenix.OdinInspector;

public class StaminaSliderUI : DoubleSliderUI
{
    [Title("Player Reference")]
    [Required, SerializeField]
    private PlayerStat _stat;

    [Title("Chunk Settings")]
    [SerializeField]
    private float _chunkValue = 20f;

    [Title("Runtime")]
    [ReadOnly]
    private float _cachedChunkIndex;

    [ReadOnly]
    private float _lastValue;

    private IReadOnlyConsumable<float> Stamina => _stat.GetConsumable(EConsumableFloat.Stamina);

    private void OnEnable()
    {
        if (Stamina == null)
        {
            _stat.OnStatInitEnd.Subscribe(UIEnable);
            return;
        }
            
        UIEnable();
    }

    private void UIEnable()
    {
        Init(Stamina.Max, Stamina.Current);
        Stamina.Subscribe(OnStaminaChanged);
        SyncInitialState();
    }

    private void OnDisable()
    {
        if (Stamina == null)
            return;

        Stamina.Unsubscribe(OnStaminaChanged);
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

    //private void OnMaxStaminaChanged(float max)
    //{
    //    SetMax(max);
    //}
}

