using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    [Header("Health")]
    public ConsumableStat<float> Health;

    [Header("Movement")]
    public ValueStat<float> MoveSpeed;

    [Header("Attack")]
    public ValueStat<float> AttackDamage;
    public ValueStat<float> AttackSpeed;
    public ValueStat<float> AttackDistance;

    [Header("Detection & Idle")]
    public ValueStat<float> DetectDistance;
    public float IdleWaitTime = 3f;

    [Header("Hit")]
    public float KnockbackTime = 0.15f;

    [Header("Patrol")]
    public float ArrivalThreshold = 2f;

    public void Init()
    {
        Health.Init(50);
        MoveSpeed.Init(5);

        AttackDamage.Init(10);
        AttackSpeed.Init(2);
        AttackDistance.Init(4f);
        DetectDistance.Init(30);
    }
}