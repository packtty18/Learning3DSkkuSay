using UnityEngine;

public interface IPoolable
{
    EPoolType PoolType { get; }
    void Get(EPoolType type);
    void Release();
}
