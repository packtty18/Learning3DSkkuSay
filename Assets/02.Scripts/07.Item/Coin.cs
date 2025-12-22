using ArtificeToolkit.Attributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Coin : MonoBehaviour, IPoolable
{
    private Rigidbody _rb;

    public EPoolType PoolType => EPoolType.Coin;

    [Header("Spawn")]
    [SerializeField] private float _spawnForce = 0.5f;
    [SerializeField] private float _spawnRotateForce = 30f;

    [Header("Detect & Attract")]
    [SerializeField] private float _detectDelay = 0.5f;
    [SerializeField] private float _attractDistance = 10f;
    [SerializeField] private float _attractSpeed = 8f;

    private Transform _player;

    private float _detectTimer;
    private bool _canDetect;
    private bool _isAttracting;
    private bool _isCollected;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    #region Pool

    public void Get(EPoolType type)
    {
        _player = PlayerController.Instance?.transform;

        ResetState();
        AddSpawnForce();

        _detectTimer = 0f;
        _canDetect = false;

        Debug.Log("Coin Get");
    }

    public void Release()
    {
        ResetPhysics();
        Debug.Log("Coin Release");
    }

    #endregion

    private void Update()
    {
        if (_player == null || _isCollected)
            return;

        // 1️⃣ 감지 지연
        if (!_canDetect)
        {
            _detectTimer += Time.deltaTime;
            if (_detectTimer >= _detectDelay)
            {
                _canDetect = true;
            }
            return;
        }

        // 2️⃣ 흡수 시작
        if (!_isAttracting &&
            Vector3.Distance(transform.position, _player.position) <= _attractDistance)
        {
            StartAttract();
        }

        // 3️⃣ 흡수 이동
        if (_isAttracting)
        {
            Vector3 dir = (_player.position - transform.position).normalized;
            transform.position += dir * _attractSpeed * Time.deltaTime;
        }
    }

    private void StartAttract()
    {
        if (_isAttracting)
            return;

        _isAttracting = true;

        // 물리 중단
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        Debug.Log("Coin Start Attract");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCollected)
            return;

        if (!other.CompareTag("Player"))
            return;

        Collect();
    }

    private void Collect()
    {
        if (_isCollected)
            return;

        _isCollected = true;

        // 점수 / 사운드 / 이펙트 처리 지점
        Util.ObjectDestroy(gameObject);
    }

    [Button]
    private void AddSpawnForce()
    {
        Vector3 dir = Vector3.up + Random.insideUnitSphere * 0.5f;
        dir.Normalize();

        _rb.AddForce(dir * _spawnForce, ForceMode.Impulse);
        _rb.angularVelocity = Random.onUnitSphere * _spawnRotateForce;
    }

    private void ResetState()
    {
        _isCollected = false;
        _isAttracting = false;

        ResetPhysics();
    }

    private void ResetPhysics()
    {
        _rb.isKinematic = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
}
