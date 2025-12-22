using UnityEngine;

public class EnemyPoolReactor : MonoBehaviour, IPoolable
{
    private EPoolType _poolType = EPoolType.Enemy;
    public EPoolType PoolType => _poolType;

    private EnemyController _controller;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    public void Get(EPoolType type)
    {
        DebugManager.Instance.Log("EnemyPoolReactor Get()");

        if (_poolType == EPoolType.None)
            _poolType = type;

        _controller.OnSpawned();
    }

    public void Release()
    {
        DebugManager.Instance.Log("EnemyPoolReactor Release()");
        _controller.OnDespawned();
    }
}
