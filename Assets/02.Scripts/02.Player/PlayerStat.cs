using System;
using UnityEngine;
using ArtificeToolkit.Attributes;

public class PlayerStat : MonoBehaviour
{
    [Title("Runtime State")]
    [ReadOnly]
    public ConsumableStat<float> Health;

    public bool IsDead { get; private set; } = false;

    public event Action OnDead;


    [Title("Move Settings")]
    [Required,SerializeField, PreviewScriptable]
    public MovementDataSO MoveData;

    [ReadOnly]
    public ConsumableStat<float> Stemina;

    [Title("Bomb Settings")]
    [Required, SerializeField, PreviewScriptable]
    public BombDataSO CurrentBombData;

    [ReadOnly]
    public ConsumableStat<int> BombCount;

    [Title("GunFire Settings")]
    [Required, SerializeField, PreviewScriptable]
    public GunDataSO CurrentGunData;

    [ReadOnly]
    public ValueStat<int> InventoryBullet;
    [ReadOnly]
    public ConsumableStat<int> LoadedBullet;
    [ReadOnly]
    public ConsumableStat<float> ReloadTimer;


    public void Awake()
    {
        Health.Init(100, 100, 1);
        IsDead = false;

        Stemina.Init(MoveData.MaxStamina, default, MoveData.StaminaRegen);

        BombCount.Init(CurrentBombData.MaxCount, default ,0.2f);

        InventoryBullet.Init(150);
        LoadedBullet.Init(CurrentGunData.MaxBullet);
        ReloadTimer.Init(CurrentGunData.ReloadTime, default, 1);

        Debug.Log("[PlayerStat] Initialized");
    }

    public void SetDead(bool tf)
    {
        if (IsDead == tf)
            return;

        IsDead = tf;
        Debug.Log("[PlayerStat] Dead State Changed");

        OnDead?.Invoke();
    }
}
