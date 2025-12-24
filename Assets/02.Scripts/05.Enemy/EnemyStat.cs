using Sirenix.OdinInspector;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    [Required]
    public EnemyStatDataSO Data;


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

    [Header("Rage")]
    public ValueStat<float> RageRate;

    public void Init()
    {
        Health.Init(Data.Health);
        MoveSpeed.Init(Data.MoveSpeed);

        AttackDamage.Init(Data.AttackDamage);
        AttackSpeed.Init(Data.AttackSpeed);
        AttackDistance.Init(Data.AttackDistance);
        DetectDistance.Init(Data.DetectDistance);

        if(Data.CanRage)
        {
            RageRate.Init(Data.RageRate);
        }
    }
}