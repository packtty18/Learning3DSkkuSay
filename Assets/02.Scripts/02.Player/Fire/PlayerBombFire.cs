using ArtificeToolkit.Attributes;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    
    [Required,SerializeField] private GameObject _firePrefab;
    [SerializeField] private Transform _firePoint;

    [Title("Runtime Cache")]
    [ReadOnly, SerializeField] private PlayerStat _stat;
    private ConsumableStat<int> _bombCount => _stat.BombCount;
    private BombDataSO _data => _stat.CurrentBombData;

    [Title("Runtime State")]
    [ReadOnly, SerializeField] private float _fireTimer;

    [SerializeField] private bool _debugFire;

    public void Init()
    {
        if (!PlayerController.IsExist())
        {
            DebugManager.Instance.Log("PlayerController is not Setted");
            return;
        }
        _stat = PlayerController.Instance.Stat;
    }

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;
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
