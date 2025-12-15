using System.Collections;
using UnityEngine;
using ArtificeToolkit.Attributes;

public class PlayerGunFire : MonoBehaviour
{
    [Title("Components")]
    
    [Required, SerializeField] private Transform _fireTransform;
    [SerializeField] private LayerMask _targetLayer;
    [Required, SerializeField] private FireRebound _bound;

    [Title("Gun Runtime Stat")]
    [SerializeField] private PlayerStat _stat;
    private GunDataSO _data => _stat.CurrentGunData;
    private ValueStat<int> _invenCount => _stat.InventoryBullet;
    private ConsumableStat<int> _currentCount => _stat.LoadedBullet;
    private ConsumableStat<float> _loadTimer => _stat.ReloadTimer;

    [Title("State")]
    [ReadOnly, SerializeField] private bool _isFireDelay = true;
    [ReadOnly, SerializeField] private bool _isReloading = false;

    private Coroutine _fireDelayRoutine;
    private Coroutine _reloadRoutine;


    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;

        if (CursorManager.Instance.IsPointerOverUI())
        {
            return;
        }

        if (_isReloading)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
            return;
        }

        if (Input.GetMouseButton(0) && !_currentCount.IsEmpty() && _isFireDelay)
        {
            Fire();
        }
    }

    private void Fire()
    {
        _isFireDelay = false;
        _fireDelayRoutine = StartCoroutine(WaitFireDelay(_data.FireDelay));

        _currentCount.Consume(1);

        Ray ray = new Ray(_fireTransform.position, CameraController.Instance.GetFireDirection(_fireTransform));
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, _data.Range, _targetLayer))
        {
            DebugManager.Instance.Log($"히트 발생!! : {hitInfo.collider.name}");

            ParticleSystem hitEffect = ParticleManager.Instance.Get(EParticleType.BulletEnvironmentHit);

            ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams
            {
                position = hitInfo.point,
                rotation3D = Quaternion.LookRotation(hitInfo.normal).eulerAngles
            };

            hitEffect.Emit(emit, 1);

            if (hitInfo.collider.TryGetComponent(out IDamageable damageable))
            {
                Vector3 knockDir = (hitInfo.collider.transform.position - _fireTransform.position).normalized;
                AttackData attackData = new AttackData(
                    _data.Damage,
                    knockDir,
                    gameObject,
                    _data.KnockbackData
                );

                damageable.ApplyDamage(attackData);
            }
        }

        _bound.PlayRebound();
    }

    private void StartReload()
    {
        if (_isReloading)
            return;

        _isReloading = true;
        _reloadRoutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        if (_invenCount.Value <= 0)
        {
            DebugManager.Instance.Log("전체 탄약이 부족하여 장전 불가!");
            _isReloading = false;
            _reloadRoutine = null;
            yield break;
        }

        DebugManager.Instance.Log("재장전 시작!");

        int targetCount = Mathf.Min(_currentCount.Max - _currentCount.Current, _invenCount.Value);

        if (targetCount <= 0)
        {
            DebugManager.Instance.Log("탄창이 이미 가득 차 있어 장전할 필요 없음!");
            _isReloading = false;
            _reloadRoutine = null;
            yield break;
        }

        _loadTimer.SetCurrent(0);

        while (!_loadTimer.IsFull())
        {
            _loadTimer.Regenerate(Time.deltaTime);
            yield return null;
        }

        _invenCount.Decrease(targetCount);
        _currentCount.SetCurrent(_currentCount.Current + targetCount);

        DebugManager.Instance.Log($"재장전 완료! +{targetCount} 장전됨");

        _isReloading = false;
        _reloadRoutine = null;
    }

    private IEnumerator WaitFireDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isFireDelay = true;
        _fireDelayRoutine = null;
    }
}
