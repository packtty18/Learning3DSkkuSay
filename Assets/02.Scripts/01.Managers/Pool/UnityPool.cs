using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UnityPool
{
    private readonly ObjectPool<GameObject> _pool;

    public UnityPool(PoolSO data, Transform parent)
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var obj = Object.Instantiate(data.PoolPrefab, parent);
                obj.SetActive(false);
                DebugManager.Instance.Log($"[Pool] Created: {data.PoolPrefab.name}");
                return obj;
            },
            actionOnGet: obj =>
            {
                obj.SetActive(true);
                if (obj.TryGetComponent(out IPoolable pool))
                {
                    pool.Get(data.PoolType);
                }
            },
            actionOnRelease: obj =>
            {
                if (obj.TryGetComponent(out IPoolable pool))
                {
                    pool.Release();
                }
                obj.SetActive(false);
            },
            actionOnDestroy: obj =>
            {
                Object.Destroy(obj);
                DebugManager.Instance.Log($"[Pool] Destroyed: {data.PoolPrefab.name}");
            },
            collectionCheck: false,
            defaultCapacity: data.DefaultCapacity,
            maxSize: data.MaxSize
        );
    }

    public void InitInstant(int count)
    {
        var tempList = new List<GameObject>(count);

        // 1) Get()만 반복 → 강제로 createFunc를 여러 번 호출
        for (int i = 0; i < count; i++)
        {
            var obj = _pool.Get();  
            tempList.Add(obj);
        }

        // 2) 다 만든 뒤 한 번에 Release
        foreach (var obj in tempList)
        {
            _pool.Release(obj);
        }

        DebugManager.Instance.Log($"[Pool] Prewarmed {count} objects.");
    }

    public GameObject Get() => _pool.Get();
    public void Release(GameObject obj) => _pool.Release(obj);
}
