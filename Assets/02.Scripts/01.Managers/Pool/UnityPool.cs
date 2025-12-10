using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UnityPool
{
    private readonly ObjectPool<GameObject> _pool;

    public UnityPool(GameObject prefab, Transform parent, int capacity, int size, int instantCount)
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var obj = Object.Instantiate(prefab, parent);
                if(obj.TryGetComponent(out IPoolable pool))
                {
                    pool.Get();
                }
                obj.SetActive(false);
                Debug.Log($"[Pool] Created: {prefab.name}");
                return obj;
            },
            actionOnGet: obj =>
            {
                obj.SetActive(true);
                if (obj.TryGetComponent(out IPoolable pool))
                {
                    pool.Get();
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
                Debug.Log($"[Pool] Destroyed: {prefab.name}");
            },
            collectionCheck: false,
            defaultCapacity: capacity,
            maxSize: size
        );

        InitInstant(instantCount);
    }

    private void InitInstant(int count)
    {
        var tempList = new List<GameObject>(count);

        // 1) Get()만 반복 → 강제로 createFunc를 여러 번 호출
        for (int i = 0; i < count; i++)
        {
            var obj = _pool.Get();   // Always creates new obj until maxSize reached
            tempList.Add(obj);
        }

        // 2) 다 만든 뒤 한 번에 Release
        foreach (var obj in tempList)
        {
            _pool.Release(obj);
        }

        Debug.Log($"[Pool] Prewarmed {count} objects.");
    }

    public GameObject Get() => _pool.Get();
    public void Release(GameObject obj) => _pool.Release(obj);
}
