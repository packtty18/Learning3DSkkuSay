using UnityEngine;
using UnityEngine.Assertions.Must;

public class Bomb : MonoBehaviour, IPoolable
{
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private LayerMask _explosionLayer;
    private PlayerStat _stat;

    private float _explosionRadius;
    private float _damage;

    public void Init(PlayerStat stat)
    {
        _stat = stat;
        _explosionRadius = _stat.ExplosionRadius.Value;
        _damage = _stat.BombDamage.Value;
    }

    public void Get()
    {
    }

    public void Release()
    {
        if (transform.TryGetComponent(out Rigidbody rigid))
        {
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
            return;

        ParticleSystem effect = ParticleManager.Instance.Get(EParticleType.BombExplosion);
        var emit = new ParticleSystem.EmitParams 
        { 
            position = transform.position 
        };
        effect.Emit(emit, 1);

        ApplyExplosionDamage(transform.position);

        gameObject.SetActive(false);
    }

    private void ApplyExplosionDamage(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, _explosionRadius, _explosionLayer);
        
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damage))
            {
                AttackData data = new AttackData(_damage, hit.transform.position, gameObject , _stat.BombKnockbackPower.Value);
                damage.ApplyDamage(data);
            }
        }
    }
}

