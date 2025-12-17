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
    public PlayerStat Stat;
    public PlayerMove Move;
    public PlayerRotate Rotate;
    public PlayerHealth Health;
    public PlayerGunFire GunFire;
    public PlayerBombFire BombFire;

    public override void Init()
    {
        CacheComponents();
    }

    
    private void CacheComponents()
    {
        Stat = GetComponent<PlayerStat>();
        Move = GetComponent<PlayerMove>();
        Rotate = GetComponent<PlayerRotate>();
        Health = GetComponent<PlayerHealth>();
        GunFire = GetComponent<PlayerGunFire>();
        BombFire = GetComponent<PlayerBombFire>();
    }
}
