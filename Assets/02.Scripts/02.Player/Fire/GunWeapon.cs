using ArtificeToolkit.Attributes;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class GunWeapon : MonoBehaviour, IWeapon
{
    [Title("Components")]
    [Required, SerializeField] private Transform _fireTransform;
    [SerializeField] private LayerMask _targetLayer;
    [Required, SerializeField] private FireRebound _rebound;

    [Title("Gun Stat")]
    [SerializeField] private PlayerStat _stat;
    private GunDataSO Data => _stat.CurrentGunData;
    private ValueStat<int> Inventory => _stat.InventoryBullet;
    private ConsumableStat<int> Loaded => _stat.LoadedBullet;
    private ConsumableStat<float> ReloadTimer => _stat.ReloadTimer;

    [Title("State")]
    [ReadOnly] private bool _canFire = true;
    [ReadOnly] private bool _isReloading;

    private Coroutine _fireDelayRoutine;
    private Coroutine _reloadRoutine;

    #region IWeapon
    public void Select()
    {
        DebugManager.Instance.Log("Gun Selected");
    }

    public void Deselect()
    {
        DebugManager.Instance.Log("Gun Deselected");
    }

    public bool CanAttack()
    {
        return _canFire && !_isReloading && !Loaded.IsEmpty();
    }

    public void Attack()
    {
        if (!CanAttack())
            return;

        _canFire = false;
        _fireDelayRoutine = StartCoroutine(FireDelay());

        Loaded.Consume(1);
        Fire();
        _rebound.PlayRebound();
    }

    public void Reload()
    {
        if (_isReloading || Inventory.Value <= 0)
            return;

        _reloadRoutine = StartCoroutine(ReloadRoutine());
    }
    #endregion

    #region Internal Logic
    private void Fire()
    {
        Ray ray = new Ray(
            _fireTransform.position,
            CameraController.Instance.GetFireDirection(_fireTransform));

        if (!Physics.Raycast(ray, out RaycastHit hit, Data.Range, _targetLayer))
            return;

        DebugManager.Instance.Log($"Hit : {hit.collider.name}");

        //파티클 로직
        ParticleSystem hitEffect = ParticleManager.Instance.Get(EParticleType.BulletEnvironmentHit);
        ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams
        {
            position = hit.point,
            rotation3D = Quaternion.LookRotation(hit.normal).eulerAngles
        };
        hitEffect.Emit(emit, 1);

        //데미지 로직
        if (hit.collider.TryGetComponent(out IDamageable damageable))
        {
            Vector3 dir = (hit.collider.transform.position - _fireTransform.position).normalized;
            damageable.ApplyDamage(new AttackData(
                Data.Damage,
                dir,
                gameObject,
                Data.KnockbackData));
        }
    }

    private IEnumerator FireDelay()
    {
        yield return new WaitForSeconds(Data.FireDelay);
        _canFire = true;
        _fireDelayRoutine = null;
    }

    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        ReloadTimer.SetCurrent(0);

        int need = Mathf.Min(
            Loaded.Max - Loaded.Current,
            Inventory.Value);

        while (!ReloadTimer.IsFull())
        {
            ReloadTimer.Regenerate(Time.deltaTime);
            yield return null;
        }

        Inventory.Decrease(need);
        Loaded.SetCurrent(Loaded.Current + need);

        DebugManager.Instance.Log($"Reloaded +{need}");

        _isReloading = false;
        _reloadRoutine = null;
    }
    #endregion
}
