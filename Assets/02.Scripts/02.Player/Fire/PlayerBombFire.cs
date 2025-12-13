using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private GameObject _firePrefab;
    [SerializeField] private Transform _firePoint;

    private ConsumableStat<int> _bombCount => _stat.BombCount;
    private BombDataSO _data => _stat.CurrentBombData;

    private float _fireTimer = 0f;      // 남은 시간 타이머

    [SerializeField] private bool _debugFire = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(2) && _debugFire)
        {
            FireBomb();
            return;
        }

        if (_bombCount.Current <= 0)
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
        _fireTimer = _data.Delay;

        _bombCount.Consume(1);

        GameObject bombObj = PoolManager.Instance.Get(EPoolType.Bomb);
        if (bombObj.TryGetComponent(out Bomb bomb))
        {
            bomb.Init(_stat.CurrentBombData);
        }

        bombObj.transform.position = _firePoint.position;
        if (bombObj.TryGetComponent(out Rigidbody rigid))
        {
            rigid.AddForce(CameraController.Instance.GetFireDirection(_firePoint) * _data.Force, ForceMode.Impulse);
        }
    }
}
