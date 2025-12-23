using UnityEngine;
using ArtificeToolkit.Attributes;

public class BombWeapon : WeaponBase
{
    [Title("References")]
    [Required, SerializeField]
    private Transform _firePoint;

    private IReadOnlyConsumable<int> _bombCount => Stat.GetConsumable(EConsumableInt.BombCount);
    private IReadOnlyValue<float> _damage => Stat.GetValue(EValueFloat.BombDamage);
    private IReadOnlyValue<float> _radius => Stat.GetValue(EValueFloat.BombRadius);
    private IReadOnlyValue<float> _force => Stat.GetValue(EValueFloat.BombThrowForce);
    private IReadOnlyValue<float> _delay => Stat.GetValue(EValueFloat.BombThrowDelay);
    private IReadOnlyValue<float> _knockbackPower => Stat.GetValue(EValueFloat.BombKnockbackPower);

    [Title("Runtime")]
    [ReadOnly]
    private float _coolTimer;

    public override bool CanAttack()
    {
        return _bombCount.Current > 0 && _coolTimer <= 0f;
    }

    public override void Attack()
    {
        if (!CanAttack())
            return;

        _coolTimer = _delay.Value;
        _bombCount.Consume(1);
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
            b.Init(_radius.Value, _damage.Value, _knockbackPower.Value);

        if (bomb.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(
                CameraController.Instance.GetFireDirection(_firePoint) * _force.Value,
                ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if (_coolTimer > 0f)
            _coolTimer -= Time.deltaTime;

        if (!_bombCount.IsFull())
            _bombCount.Regenerate(Time.deltaTime);
    }
}
