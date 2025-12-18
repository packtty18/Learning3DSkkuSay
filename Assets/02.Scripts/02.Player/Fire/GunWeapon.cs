using UnityEngine;
using System.Collections;
using ArtificeToolkit.Attributes;

public class GunWeapon : WeaponBase
{
    [Required, SerializeField] private Transform _fireTransform;
    [Required,SerializeField] private Transform _muzzleTransform;
    [SerializeField] private LayerMask _targetLayer;
    [Required, SerializeField] private FireRebound _rebound;
    
    private GunDataSO Data => Stat.CurrentGunData;
    private ValueStat<int> Inventory => Stat.InventoryBullet;
    private ConsumableStat<int> Loaded => Stat.LoadedBullet;
    private ConsumableStat<float> ReloadTimer => Stat.ReloadTimer;

    private bool _canFire = true;
    private bool _isReloading;

    public override bool CanAttack()
    {
        return _canFire && !_isReloading && !Loaded.IsEmpty();
    }

    public override void Attack()
    {
        if (!CanAttack())
            return;

        _canFire = false;
        Loaded.Consume(1);
        _rebound.PlayRebound();
        Animator.SetTrigger("Attack");
        StartCoroutine(FireDelay());
    }

    public override void AttackEnd()
    {
    }

    public override void Reload()
    {
        if (_isReloading || Inventory.Value <= 0)
            return;

        StartCoroutine(ReloadRoutine());
    }

    public void GunFire()
    {
        Muzzle muzzle = PoolManager.Instance.Get(EPoolType.Muzzle).GetComponent<Muzzle>();
        muzzle.transform.position = _muzzleTransform.position;
        muzzle.transform.rotation = _muzzleTransform.rotation;

        Ray ray = new Ray(
            _fireTransform.position,
            CameraController.Instance.GetFireDirection(_fireTransform));

        if (!Physics.Raycast(ray, out RaycastHit hit, Data.Range, _targetLayer))
            return;

        ParticleSystem hitEffect = ParticleManager.Instance.Get(EParticleType.BulletEnvironmentHit);
        ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams
        {
            position = hit.point,
            rotation3D = Quaternion.LookRotation(hit.normal).eulerAngles
        };
        hitEffect.Emit(emit, 1);

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

        _isReloading = false;
    }
}
