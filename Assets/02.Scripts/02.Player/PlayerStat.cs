using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    
    public ConsumableStat<float> Health;
    public bool IsDead { get; private set; } = false;

    [Header("Move Settings")]
    public ValueStat<float> MoveSpeed;
    public ValueStat<float> DashSpeed;
    public ValueStat<float> JumpPower;

    public ConsumableStat<float> Stemina;
    public ValueStat<float> SteminaRegenDelay; // 회복 딜레이
    public ValueStat<float> DashConsume; // 달릴 때 초당 소비
    public ValueStat<float> DoubleJumpConsume; // 이단점프 시 소비 => 스태미나 모션 기준

    [Header("Bomb Settings")]
    public ValueStat<float> BombDamage;
    public ValueStat<float> ExplosionRadius;
    public ValueStat<float> BombKnockbackPower;

    public ConsumableStat<int> BombCount;
    public ValueStat<float> BombForce;
    public ValueStat<float> BombDelay;


    [Header("GunFire Settings")]
    public ValueStat<int> EntireBullet;           //현재 총이 사용 가능한 총알의 개수
    public ConsumableStat<int> LoadedBulletCount; //현재  장전된 총알
    public ValueStat<float> GunDamage;            //총의 데미지
    public ValueStat<float> FireDelay;            //총의 발사 간격
    public ValueStat<float> GunRange;             //총의 사거리
    public ConsumableStat<float> ReloadTime;      //재장전 시간
    public ValueStat<float> GunKnockbackPower;

    private void Awake()
    {
        Health.Init(100, 100, 1);
        IsDead = false;

        MoveSpeed.Init(5);
        DashSpeed.Init(10);
        JumpPower.Init(5);

        Stemina.Init(100, 100, 10);
        SteminaRegenDelay.Init(3);
        DashConsume.Init(5);
        DoubleJumpConsume.Init(20);

        BombDamage.Init(20);
        ExplosionRadius.Init(5);
        BombKnockbackPower.Init(30);

        BombCount.Init(5, 5, 0);
        BombForce.Init(15f);
        BombDelay.Init(1f);

        EntireBullet.Init(150);
        LoadedBulletCount.Init(30, 30, 0);
        GunDamage.Init(10);
        FireDelay.Init(0.1f);
        GunRange.Init(30);
        ReloadTime.Init(1.6f, 1.6f, 1);
        GunKnockbackPower.Init(5);
    }


    public void OnDead(bool tf)
    {
        IsDead = tf;
    }
}
