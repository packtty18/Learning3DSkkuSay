using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public ConsumableStat<float> Health;
    public bool IsDead { get; private set; } = false;

    [Header("Move Settings")]
    public MovementDataSO MoveData;
    public ConsumableStat<float> Stemina;

    [Header("Bomb Settings")]
    public BombDataSO CurrentBombData;
    public ConsumableStat<int> BombCount;

    [Header("GunFire Settings")]
    public GunDataSO CurrentGunData;
    public ValueStat<int> InventoryBullet;
    public ConsumableStat<int> LoadedBullet;
    public ConsumableStat<float> ReloadTimer;

    private void Awake()
    {
        Health.Init(100, 100, 1);
        IsDead = false;

        Stemina.Init(MoveData.MaxStamina, default, MoveData.StaminaRegen);

        BombCount.Init(CurrentBombData.MaxCount);

        InventoryBullet.Init(150);
        LoadedBullet.Init(CurrentGunData.MaxBullet);
        ReloadTimer.Init(CurrentGunData.ReloadTime, default, 1);
    }
    public void OnDead(bool tf)
    {
        IsDead = tf;
    }
}
