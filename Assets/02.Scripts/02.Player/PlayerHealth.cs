using UnityEngine;
using ArtificeToolkit.Attributes;
using TMPro.EditorUtilities;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    [Title("References")]
    [SerializeField] private PlayerController _controller;
    private Animator _animator => _controller.Animator;
     private PlayerStat _stat => _controller.Stat;
    private ConsumableStat<float> Health => _stat.Health;

    public void ApplyDamage(AttackData data)
    {
        if (_stat.IsDead)
            return;

        Health.Consume(data.Damage);
        _animator.SetLayerWeight(3, (1- Health.Current / Health.Max));
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
