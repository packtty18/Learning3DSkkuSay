using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum EPoolType
{
    Bomb,
    Explosion
}

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<EPoolType, UnityPool> _pools =new Dictionary<EPoolType, UnityPool>();
    [SerializeField] private Transform _roots;
    [SerializeField] private List<PoolSO> _poolDatas;
    public override void Init()
    {
        _pools = new Dictionary<EPoolType, UnityPool>();
        if(_roots == null)
        {
            GameObject root = new GameObject("PoolRoots");
            _roots = root.GetComponent<Transform>();
        }

        for (int i =0; i< _poolDatas.Count; i++)
        {
            if (_pools.TryGetValue(_poolDatas[i]._poolType, out _))
            {
                Debug.LogWarning($"[PoolManager] Duplicate prefab detected: {_poolDatas[i].PoolPrefab.name}. Skipped.");
                continue;
            }
            MakePool(_poolDatas[i]);
        }
    }
    private UnityPool MakePool(PoolSO poolData)
    {
        UnityPool pool = new UnityPool(poolData.PoolPrefab, _roots, poolData.DefaultCapacity,poolData.MaxSize ,poolData.DefaultInstantCount);
        _pools[poolData._poolType] = pool;
        return pool;
    }

    public GameObject Get(EPoolType key)
    {
        if (!_pools.TryGetValue(key, out var pool))
        {
            DebugManager.Instance.Log($"[PoolManager] doesnt exist: {name}");
            return null;
        }

        return pool.Get();
    }

    public void Release(EPoolType key, GameObject instance)
    {
        if (_pools.TryGetValue(key, out var pool))
        {
            pool.Release(instance);
        }
        else
        {
            DebugManager.Instance.LogWarning($"[PoolManager] No pool exists for {name}. Destroy object.");
            Destroy(instance);
        }
    }
}
