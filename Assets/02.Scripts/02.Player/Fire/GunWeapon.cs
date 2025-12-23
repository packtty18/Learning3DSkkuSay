using UnityEngine;
using System.Collections;
using ArtificeToolkit.Attributes;
using Unity.Android.Gradle.Manifest;
using UnityEditor.ShaderGraph.Internal;

public class GunWeapon : WeaponBase
{
    [Required, SerializeField] private Transform _fireTransform;
    [Required,SerializeField] private Transform _muzzleTransform;
    [SerializeField] private LayerMask _targetLayer;
    [Required, SerializeField] private FireRebound _rebound;
    
    private GunDataSO Data => Stat.CurrentGun;
    private IReadOnlyValue<int> Inventory => Stat.InventoryBullet;
    private IReadOnlyConsumable<int> Loaded => Stat.LoadedBullet;

    private float _reloadTime => Data.ReloadTime;
    private float _reloadTimer = 0;

    public float ReloadTime => _reloadTime;
    public float ReloadTimer => _reloadTimer;

    private bool _canFire = true;
    private bool _isReloading;

    public SafeEvent<float> OnReloadTimerChange;
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

        BulletTrail trail = PoolManager.Instance.Get(EPoolType.BulletTrail).GetComponent<BulletTrail>();
        trail.transform.position = _muzzleTransform.position;
        trail.transform.rotation = _muzzleTransform.rotation;

        if (Physics.Raycast(ray, out RaycastHit hit, Data.Range, _targetLayer))
        {
            trail.SetDestination(hit.point);

            // Hit effect
            ParticleSystem hitEffect =
                ParticleManager.Instance.Get(EParticleType.BulletEnvironmentHit);

            ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams
            {
                position = hit.point,
                rotation3D = Quaternion.LookRotation(hit.normal).eulerAngles
            };
            hitEffect.Emit(emit, 1);

            // Damage
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                Vector3 dir =
                    (hit.point - _fireTransform.position).normalized;

                damageable.ApplyDamage(new AttackData(
                    Data.Damage,
                    dir,
                    gameObject,
                    Data.KnockbackData));
            }
        }
        else
        {
            Vector3 missPoint = ray.origin + ray.direction * Data.Range;
            trail.SetDestination(missPoint);
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
        _reloadTimer = ReloadTime;
        

        int need = Mathf.Min(
            Loaded.Max - Loaded.Current,
            Inventory.Value);

        while (_reloadTimer > 0)
        {
            _reloadTimer -= Time.deltaTime;
            OnReloadTimerChange.Invoke(_reloadTimer);
            yield return null;
        }

        _reloadTimer = 0;
        Stat.SetReloadBullet(need, Loaded.Current + need);

        _isReloading = false;
    }
}
