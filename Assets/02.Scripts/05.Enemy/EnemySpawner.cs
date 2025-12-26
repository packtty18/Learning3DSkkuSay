using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : SerializedMonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private int _spawnCount = 3;
    [SerializeField] private float _respawnDelay = 1f;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] _patrolPoints;

    [SerializeField]
    private readonly HashSet<EnemyController> _aliveEnemies = new();
    private Coroutine _respawnCoroutine;

    public EPoolType PoolType = EPoolType.Enemy1;

    private void Start()
    {
        StartCoroutine(SpawnAfterDelay());
    }

    private void SpawnEnemy()
    {
        GameObject pool = PoolManager.Instance.Get(PoolType);

        if(!pool.TryGetComponent(out EnemyController enemy))
        {
            return;
        }
        enemy.transform.position = transform.position;
        enemy.SetSpawnPoint(transform);
        enemy.SetPatrolPoints(_patrolPoints);

        enemy.OnReturnedToPool+= OnEnemyReturned;
        _aliveEnemies.Add(enemy);

        DebugManager.Instance.Log(
            $"[Spawner] Enemy Spawned ({_aliveEnemies.Count}/{_spawnCount})");
    }

    private void OnEnemyReturned(EnemyController enemy)
    {
        enemy.OnReturnedToPool -= OnEnemyReturned;
        _aliveEnemies.Remove(enemy);

        if (_respawnCoroutine == null)
        {
            _respawnCoroutine = StartCoroutine(RespawnAfterDelay());
        }
    }
    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitUntil(() => GameManager.Instance.State == EGameState.Playing);

        while (_aliveEnemies.Count < _spawnCount)
        {
            SpawnEnemy();
        }
    }
    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitUntil(() => GameManager.Instance.State == EGameState.Playing);

        while (_aliveEnemies.Count < _spawnCount)
        {
            yield return new WaitForSeconds(_respawnDelay);
            SpawnEnemy();
        }

        _respawnCoroutine = null;
    }
}
