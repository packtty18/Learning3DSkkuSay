using UnityEngine;
using ArtificeToolkit.Attributes;

public class BombWeapon : WeaponBase
{
    [Required, SerializeField] private Transform _firePoint;

    private BombDataSO _data => Stat.CurrentBomb;
    private float _coolTimer;

    public override bool CanAttack()
    {
        return Stat.BombCount.Current > 0 && _coolTimer <= 0f;
    }

    public override void Attack()
    {
        if (!CanAttack())
            return;

        _coolTimer = _data.Delay;
        Stat.BombCount.Consume(1);
        Animator.SetTrigger("Throw");
    }

    public override void AttackEnd()
    {
        // 폭탄은 없음
    }

    public override void Reload()
    {
        // 폭탄은 없음
    }

    public void BombFire()
    {
        GameObject bomb = PoolManager.Instance.Get(EPoolType.Bomb);
        bomb.transform.position = _firePoint.position;

        if (bomb.TryGetComponent(out Bomb b))
            b.Init(_data);

        if (bomb.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(
                CameraController.Instance.GetFireDirection(_firePoint) * _data.Force,
                ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if (_coolTimer > 0f)
            _coolTimer -= Time.deltaTime;

        if (!Stat.BombCount.IsFull())
            Stat.BombCount.Regenerate(Time.deltaTime);
    }
}
