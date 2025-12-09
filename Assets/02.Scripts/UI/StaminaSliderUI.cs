using UnityEngine;

public class StaminaSliderUI : DoubleSliderUI
{
    [Header("Player Reference")]
    [SerializeField] private PlayerMove player;

    private void Start()
    {
        Init(player.MaxStamina, player.CurrentStamina);
    }

    private void Update()
    {
        SetValue(player.CurrentStamina);
    }
}
