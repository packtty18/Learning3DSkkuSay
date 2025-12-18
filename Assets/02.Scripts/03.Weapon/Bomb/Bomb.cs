using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Bomb : MonoBehaviour, IPoolable
{
    private EPoolType _type = EPoolType.None;
    public EPoolType PoolType => _type;


    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private LayerMask _explosionLayer;
    [SerializeField] private float _delayTime = 3f;
    private float _explosionRadius;
    private float _damage;
    private KnockbackData _knockback;

    private Coroutine _coroutine;
    
    public void Init(BombDataSO data)
    {
        _explosionRadius = data.ExplosionRadius;
        _damage = data.Damage;
        _knockback = data.Knockback;
    }

    public void Get(EPoolType type)
    {
        if(_type == EPoolType.None)
        {
            _type = type;
        }

        _coroutine = StartCoroutine(Explosion());
    }

    public void Release()
    {
        if (transform.TryGetComponent(out Rigidbody rigid))
        {
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds( _delayTime );  
        ParticleSystem effect = ParticleManager.Instance.Get(EParticleType.BombExplosion);
        var emit = new ParticleSystem.EmitParams
        {
            position = transform.position,
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
                AttackData data = new AttackData(_damage, dir, gameObject , _knockback);
                damage.ApplyDamage(data);
            }
        }
    }
}

