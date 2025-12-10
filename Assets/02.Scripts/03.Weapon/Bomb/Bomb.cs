using UnityEngine;

public class Bomb : MonoBehaviour, IPoolable
{
    [SerializeField] private GameObject _explosionPrefab;

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
        if(collision.transform.CompareTag("Player"))
        {
            return;
        }

        ParticleSystem effect = ParticleManager.Instance.Get(EParticleType.BombExplosion);
        ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();
        emit.position = transform.position;
        effect.Emit(emit, 1);
        PoolManager.Instance.Release(EPoolType.Bomb,gameObject);
    }
}
