using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerStat _stat;
    private ConsumableStat<float> _health => _stat.Health;


    private void Awake()
    {

    }

    private void PlayHitEffect()
    {
        //추후 추가
    }

    private void Die()
    {
        _stat.OnDead(true);
    }

    public void ApplyDamage(AttackData data)
    {
        if (_stat.IsDead)
        {
            return;
        }

        _health.Consume(data.Damage);
        if (_health.IsEmpty())
        {
            Die();
        }
        else
        {
            PlayHitEffect();
        }
    }
}
