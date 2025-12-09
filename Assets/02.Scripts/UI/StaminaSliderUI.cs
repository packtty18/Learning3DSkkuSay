using MoreMountains.Feedbacks;
using UnityEngine;

public class StaminaSliderUI : DoubleSliderUI
{
    [Header("Player Reference")]
    [SerializeField] private PlayerStat _stat;

    private ConsumableStat _stemina => _stat.Stemina;

    private void Start()
    {
        Init(_stemina.MaxValue, _stemina.CurrentValue);
    }

    private void Update()
    {
        SetValue(_stemina.CurrentValue);
    }
}
