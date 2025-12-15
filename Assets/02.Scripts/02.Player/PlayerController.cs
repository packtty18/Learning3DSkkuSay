using UnityEngine;
using ArtificeToolkit.Attributes;

[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerRotate))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerGunFire))]
[RequireComponent(typeof(PlayerBombFire))]
public class PlayerController : Singleton<PlayerController>
{
    [Title("Cached Components (Read Only)")]
    [ReadOnly, SerializeField] private PlayerStat _stat;
    [ReadOnly, SerializeField] private PlayerMove _move;
    [ReadOnly, SerializeField] private PlayerRotate _rotate;
    [ReadOnly, SerializeField] private PlayerHealth _health;
    [ReadOnly, SerializeField] private PlayerGunFire _gunFire;
    [ReadOnly, SerializeField] private PlayerBombFire _bombFire;

    public PlayerStat Stat => _stat;
    public PlayerMove Move => _move;
    public PlayerRotate Rotate => _rotate;
    public PlayerHealth Health => _health;
    public PlayerGunFire GunFire => _gunFire;
    public PlayerBombFire BombFire => _bombFire;


    public override void Init()
    {
        CacheComponents();
    }

    
    private void CacheComponents()
    {
        _stat = GetComponent<PlayerStat>();
        _move = GetComponent<PlayerMove>();
        _rotate = GetComponent<PlayerRotate>();
        _health = GetComponent<PlayerHealth>();
        _gunFire = GetComponent<PlayerGunFire>();
        _bombFire = GetComponent<PlayerBombFire>();
    }
}
