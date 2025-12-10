using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private GameObject _firePrefab;
    [SerializeField] private Transform _firePoint;

    private ConsumableStat<int> _bombCount => _stat.BombCount;
    [SerializeField] private ValueStat<float> _bombForce => _stat.BombForce;
    [SerializeField] private ValueStat<float> _bombDelay => _stat.BombDelay;

    private float _fireTimer = 0f;      // 남은 시간 타이머

    [SerializeField] private bool _debugFire = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(2) && _debugFire)
        {
            FireBomb();
            return;
        }

        if (_bombCount.CurrentValue <= 0)
        {
            return;
        }

        if (_fireTimer > 0f)
        {
            _fireTimer -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(2) && _fireTimer <= 0f)
        {
            FireBomb();
        }
    }

    private void FireBomb()
    {
        _fireTimer = _bombDelay.Value;

        _bombCount.DecreaseCurrent(1);

        GameObject bomb = PoolManager.Instance.Get(EPoolType.Bomb);
        bomb.transform.position = _firePoint.position;
        bomb.transform.rotation = _firePoint.rotation;
        if (bomb.TryGetComponent(out Rigidbody rigid))
        {
            rigid.AddForce(_firePoint.forward * _bombForce.Value, ForceMode.Impulse);
        }
    }
}
