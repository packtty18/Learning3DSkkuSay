using UnityEngine;

[CreateAssetMenu(fileName = "PoolSO", menuName = "ScriptableObjects/PoolSO")]
public class PoolSO : ScriptableObject
{
    public EPoolType _poolType;
    public GameObject PoolPrefab;
    public int DefaultInstantCount = 5;
    public int DefaultCapacity= 10;
    public int MaxSize = 20;
}
