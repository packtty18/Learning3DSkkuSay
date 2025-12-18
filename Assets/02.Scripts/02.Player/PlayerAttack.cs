using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private WeaponBase[] _weapons;

    [SerializeField] private PlayerController _controller;
    private WeaponBase _current;
    private int _index;


    private void Start()
    {
        ChangeWeapon(0);
    }
    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;

        if (Input.GetMouseButton(0))
            TryAttack();

        if (Input.GetMouseButtonUp(0))
            _current?.AttackEnd();

        if (Input.GetKeyDown(KeyCode.R))
            _current?.Reload();

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
            ChangeWeapon((_index + (scroll > 0 ? 1 : -1) + _weapons.Length) % _weapons.Length);
    }

    private void TryAttack()
    {
        if (_current == null)
            return;

        if (_current.CanAttack())
            _current.Attack();
    }
    private void ChangeWeapon(int idx)
    {
        _index = idx;
        _current = _weapons[_index];

        DebugManager.Instance.Log($"Weapon Changed : {_current.name}");
    }

    public void OnFireEvent()
    {
        if (_current is GunWeapon gun)
            gun.GunFire();
    }

    public void OnThrowEvent()
    {
        if (_current is BombWeapon bomb)
            bomb.BombFire();
    }

}
