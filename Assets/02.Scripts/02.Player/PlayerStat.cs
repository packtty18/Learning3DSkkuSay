using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public ConsumableStat Stemina;
    public ConsumableStat Health;
    public ConsumableStat BombCount;

    public ValueStat Damage;
    public ValueStat AttackSpeed;

    public ValueStat MoveSpeed;
    public ValueStat DashSpeed;
    public ValueStat JumpPower;

    [Header("Stamina Settings")]
    public ValueStat SteminaRegenDelay; // 회복 딜레이
    public ValueStat DashConsume; // 달릴 때 초당 소비
    public ValueStat DoubleJumpConsume; // 이단점프 시 소비 => 스태미나 모션 기준

    [Header("FireBomb Settings")]
    public ValueStat BombForce;
    public ValueStat BombDelay;


    private void Awake()
    {
        Stemina.Init(100,100,10);
        Health.Init(100,100,1);
        BombCount.Init(5, 5, 0);

        Damage.Init(10);
        AttackSpeed.Init(2);
        MoveSpeed.Init(5);
        DashSpeed.Init(10);
        JumpPower.Init(5);

        SteminaRegenDelay.Init(3);
        DashConsume.Init(5);
        DoubleJumpConsume.Init(20);

        BombForce.Init(15f);
        BombDelay.Init(1f);
    }
}
