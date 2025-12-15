using UnityEngine;
using ArtificeToolkit.Attributes;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    [Title("References")]
    [SerializeField] private PlayerStat _stat;
    private ConsumableStat<float> Health => _stat.Health;

    public void ApplyDamage(AttackData data)
    {
        if (_stat.IsDead)
            return;

        Health.Consume(data.Damage);

        if (Health.IsEmpty())
        {
            Die();
        }
        else
        {
            PlayHitEffect();
        }
    }

    private void PlayHitEffect()
    {
        Debug.Log("[PlayerHealth] Hit");
    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] Die");
        _stat.SetDead(true);
    }
}
