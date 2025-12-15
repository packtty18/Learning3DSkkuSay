using System.Collections;
using UnityEngine;

public class Barrel : MonoBehaviour, IDamageable
{
    public ConsumableStat<float> Health;
    public ValueStat<float> Damage;
    public ValueStat<float> ExplosionRange;
    public ValueStat<float> KnockbackPower;
    [SerializeField] private LayerMask _explosionLayer;

    [SerializeField] private float _explosionPower = 30f;

    private Rigidbody _rigid;
    private bool _isDead = false;

    

    private void Start()
    {
        Health.Init(50, 50,0);
        Damage.Init(50);
        ExplosionRange.Init(3);
        KnockbackPower.Init(30);
        _explosionPower = 30f;

        _rigid = GetComponent<Rigidbody>();
        _rigid = GetComponent<Rigidbody>();
    }

    public void ApplyDamage(AttackData data)
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;

        if (_isDead) return;

        Health.Consume(data.Damage);

        if (Health.IsEmpty())
        {
            _isDead = true;
            Explosion(data.HitDirection);
        }
        else
        {
            Knockback();
        }
    }

    private void Knockback()
    {
        //넉백을 줄까? 말까?
    }

    private void Explosion(Vector3 hitDir)
    {
        //파티클
        ParticleSystem effect = ParticleManager.Instance.Get(EParticleType.BombExplosion);
        ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams
        {
            position = transform.position
        };
        effect.Emit(emit, 1);


        //위로 솟아오르게
        Vector3 explosionDir = (hitDir.normalized + Vector3.up * 0.5f).normalized;
        // 회전이 생기도록 힘을 비대칭 지점에 적용
        Vector3 forcePoint = transform.position + Random.insideUnitSphere * 0.4f;

        _rigid.AddForceAtPosition(explosionDir * _explosionPower, forcePoint, ForceMode.Impulse);


        //폭발 반경 데미지 주기
        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRange.Value, _explosionLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable target))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                KnockbackData knockback = new KnockbackData(KnockbackPower.Value);
                AttackData data = new AttackData(
                    Damage.Value,
                    dir,
                    gameObject,
                    knockback
                );

                target.ApplyDamage(data);
            }
        }
        StartCoroutine(Util.DestroyAfterTime(5, gameObject));
    }
}
