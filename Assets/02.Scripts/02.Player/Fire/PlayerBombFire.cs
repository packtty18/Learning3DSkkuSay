using UnityEngine;
using ArtificeToolkit.Attributes;

public class PlayerBombFire : MonoBehaviour
{
    [Required, SerializeField] private GameObject _firePrefab;
    [SerializeField] private Transform _firePoint;

    [Title("Runtime Cache")]
    [SerializeField] private PlayerStat _stat;
    private ConsumableStat<int> _bombCount => _stat.BombCount;
    private BombDataSO _data => _stat.CurrentBombData;

    [Title("Runtime State")]
    [ReadOnly, SerializeField] private float _fireTimer;

    [SerializeField] private bool _debugFire;

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;

        // FPS / BackView에서만 폭탄 발사
        if ((CameraController.Instance.CurrentMode == CameraMode.FPS ||
             CameraController.Instance.CurrentMode == CameraMode.BackView) &&
            (Input.GetMouseButtonDown(2) || _debugFire))
        {
            FireBomb();
        }

        if (_bombCount.Current <= 0)
            return;

        if (_fireTimer > 0f)
            _fireTimer -= Time.deltaTime;
    }

    public void FireBomb()
    {
        if (_bombCount.Current <= 0 || _fireTimer > 0f)
            return;

        _fireTimer = _data.Delay;
        _bombCount.Consume(1);

        GameObject bombObj = PoolManager.Instance.Get(EPoolType.Bomb);
        if (bombObj.TryGetComponent(out Bomb bomb))
            bomb.Init(_data);

        bombObj.transform.position = _firePoint.position;
        if (bombObj.TryGetComponent(out Rigidbody rigid))
        {
            rigid.AddForce(CameraController.Instance.GetFireDirection(_firePoint) * _data.Force, ForceMode.Impulse);
        }
    }
}
