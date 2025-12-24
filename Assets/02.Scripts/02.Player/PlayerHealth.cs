using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Title("References")]
    [Required, SerializeField]
    private PlayerController _controller;

    private Animator _animator => _controller.Animator;
    private PlayerStat _stat => _controller.Stat;
    private IReadOnlyConsumable<float> Health => _stat.GetConsumable(EConsumableFloat.Health);

    public void ApplyDamage(AttackData data)
    {
        if (_stat.IsDead)
            return;

        Health.Consume(data.Damage);

        _animator.SetLayerWeight(3, 1f - (Health.Current / Health.Max));

        if (Health.IsEmpty())
        {
            _animator.SetTrigger("Dead");
            Die();
        }
        else
        {
            _animator.SetTrigger("Hit");
            PlayHitEffect();
        }
    }

    private void PlayHitEffect()
    {
        DebugManager.Instance.Log("[PlayerHealth] Hit");
    }

    private void Die()
    {
        DebugManager.Instance.Log("[PlayerHealth] Die");
        _stat.SetDead(true);
    }
}
