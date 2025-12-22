using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private int _spawnCount = 3;
    [SerializeField] private float _respawnDelay = 1f;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] _patrolPoints;

    private readonly HashSet<EnemyController> _aliveEnemies = new();
    private Coroutine _respawnCoroutine;

    private void Start()
    {
        _respawnCoroutine = StartCoroutine(RespawnAfterDelay());
    }

    private void SpawnEnemy()
    {
        EnemyController enemy =
            PoolManager.Instance.Get(EPoolType.Enemy)
            .GetComponent<EnemyController>();
        enemy.SetSpawnPoint(transform.position);
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
