using System.Collections;
using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private FireRebound _bound;

    [Header("GunStat")]
    [SerializeField] private ValueStat<int> _entireBulletCount => _stat.EntireBullet;
    [SerializeField] private ConsumableStat<int> _loadedBulletCount => _stat.LoadedBulletCount;    
    [SerializeField] private ValueStat<float> _gunRange => _stat.GunRange;      
    [SerializeField] private ValueStat<float> _fireDelay => _stat.FireDelay;        
    [SerializeField] private ConsumableStat<float> _reloadTime => _stat.ReloadTime;      

    private bool _isFireDelay = true;
    private bool _isReloading = false;

    private Coroutine _fireDelayRoutine;
    private Coroutine _reloadRoutine;

    private void Update()
    {
        // 재장전 중이면 아무 행동 불가
        if (_isReloading)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
            return;
        }

        if (Input.GetMouseButton(0) && !_loadedBulletCount.IsEmpty()&&  _isFireDelay)
        {
            Fire();
        }
    }

    private void Fire()
    {
        _isFireDelay = false;
        _fireDelayRoutine = StartCoroutine(WaitFireDelay(_fireDelay.Value));

        // 탄약 소모
        _loadedBulletCount.Consume(1);

        Ray ray = new Ray(_fireTransform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, _gunRange.Value, _targetLayer))
        {
            DebugManager.Instance.Log($"히트 발생!! : {hitInfo.collider.name}");

            ParticleSystem hitEffect = ParticleManager.Instance.Get(EParticleType.BulletEnvironmentHit);

            ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();
            emit.position = hitInfo.point;
            emit.rotation3D = Quaternion.LookRotation(hitInfo.normal).eulerAngles;

            hitEffect.Emit(emit, 1);
        }

        _bound.PlayRebound();
    }


    private void StartReload()
    {
        if (_isReloading) return;

        _isReloading = true;
        _reloadRoutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        //전체 탄약이 0이면 장전 불가
        if (_entireBulletCount.Value <= 0)
        {
            DebugManager.Instance.Log("전체 탄약이 부족하여 장전 불가!");
            _isReloading = false;
            _reloadRoutine = null;
            yield break;
        }

        DebugManager.Instance.Log("재장전 시작!");

        // 재장전해야 할 양 계산
        int targetCount = Mathf.Min(_loadedBulletCount.MaxValue - _loadedBulletCount.CurrentValue, _entireBulletCount.Value);
        // 장전할 탄약이 0이라면 중단
        if (targetCount <= 0)
        {
            DebugManager.Instance.Log("탄창이 이미 가득 차 있어 장전할 필요 없음!");
            _isReloading = false;
            _reloadRoutine = null;
            yield break;
        }

        _reloadTime.SetCurrentValue(0);
        while (!_reloadTime.IsFull())
        {
            _reloadTime.Regenerate();
            yield return null;
        }

        _entireBulletCount.Decrease(targetCount);
        _loadedBulletCount.IncreaseCurrent(targetCount);

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
