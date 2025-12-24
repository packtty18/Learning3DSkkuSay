using Sirenix.OdinInspector;
using DG.Tweening;
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

    [Header("State")]
    [ReadOnly] public bool IsGround;

    private Transform _player;
    private float _spawnTime;

    private bool _isAttracting;
    private Tween _attractTween;

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

        _spawnTime = Time.time;
        IsGround = false;

        Debug.Log("Coin Get");
    }

    public void Release()
    {
        KillTween();

        _isAttracting = false;
        IsGround = false;

        ResetPhysics();

        Debug.Log("Coin Release");
    }

    #endregion

    private void Update()
    {
        if (_player == null)
            return;

        if (_isAttracting)
            return;

        if (!IsGround)
            return;

        // 스폰 직후 바로 끌려오지 않도록 딜레이
        if (Time.time - _spawnTime < _detectDelay)
            return;

        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance <= _attractDistance)
        {
            StartAttract();
        }
    }

    #region Collision / Trigger

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
            return;

        IsGround = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Collect();
    }

    #endregion

    #region Attract Logic

    private void StartAttract()
    {
        _isAttracting = true;

        // 물리 영향 제거
        _rb.isKinematic = true;

        Debug.Log("Coin Attract Start");

        AttractStep();
    }

    private void AttractStep()
    {
        if (_player == null)
            return;

        float distance = Vector3.Distance(transform.position, _player.position);

        float duration = Mathf.Clamp(
            distance / _attractSpeed,
            0.05f,
            0.15f
        );

        _attractTween = transform
            .DOMove(_player.position, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                if (_isAttracting)
                {
                    AttractStep();
                }
            });
    }

    #endregion

    #region Collect

    private void Collect()
    {
        KillTween();

        Util.ObjectDestroy(gameObject);
    }

    #endregion

    #region Utility

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
        KillTween();
        ResetPhysics();
    }

    private void ResetPhysics()
    {
        _rb.isKinematic = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    private void KillTween()
    {
        if (_attractTween != null)
        {
            _attractTween.Kill();
            _attractTween = null;
        }
    }

    #endregion
}
