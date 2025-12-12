using UnityEngine;
using UnityEngine.Assertions.Must;

public class Bomb : MonoBehaviour, IPoolable
{
    private EPoolType _type = EPoolType.None;
    public EPoolType PoolType => _type;


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

    public void Get(EPoolType type)
    {
        if(_type == EPoolType.None)
        {
            _type = type;
        }
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

        Util.ObjectDestroy(gameObject);
    }

    private void ApplyExplosionDamage(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, _explosionRadius, _explosionLayer);
        
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damage))
            {
                Vector3 dir = (hit.transform.position - center).normalized;
                AttackData data = new AttackData(_damage, dir, gameObject , _stat.BombKnockbackPower.Value);
                damage.ApplyDamage(data);
            }
        }
    }
}

