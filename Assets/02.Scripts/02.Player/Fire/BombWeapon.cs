using UnityEngine;
using ArtificeToolkit.Attributes;
using Unity.VisualScripting;

public class BombWeapon : MonoBehaviour, IWeapon
{
    [Required, SerializeField] private Transform _firePoint;
    [SerializeField] private PlayerStat _stat;

    private BombDataSO Data => _stat.CurrentBombData;
    private ConsumableStat<int> Count => _stat.BombCount;

    [ReadOnly] private float _coolTimer;


    public void Select()
    {
        DebugManager.Instance.Log("Bomb Selected");
    }

    public void Deselect()
    {
        DebugManager.Instance.Log("Bomb Deselected");
    }

    public bool CanAttack()
    {
        return Count.Current > 0 && _coolTimer <= 0f;
    }

    public void Attack()
    {
        if (!CanAttack())
            return;

        _coolTimer = Data.Delay;
        Count.Consume(1);

        GameObject bomb = PoolManager.Instance.Get(EPoolType.Bomb);
        bomb.transform.position = _firePoint.position;

        if (bomb.TryGetComponent(out Bomb b))
            b.Init(Data);

        if (bomb.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(
                CameraController.Instance.GetFireDirection(_firePoint) * Data.Force,
                ForceMode.Impulse);
        }
    }

    public void Reload()
    {
        // 폭탄은 재장전 없음
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        if (_coolTimer > 0f)
            _coolTimer -= delta;

        if (!Count.IsFull())
        {
            Count.Regenerate(delta);
        }
    }
}
