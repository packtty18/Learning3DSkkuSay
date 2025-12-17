using UnityEngine;
using ArtificeToolkit.Attributes;
using System.Collections;

//책임 : 인풋, 현재 사용할 무기 판정, 현재 무기의 리로딩, 딜레이 계산, 발사 가능여부 계산, 발사 전달
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] _weaponSlots;

    private IWeapon[] _weapons;
    private IWeapon _current;
    private int _index;

    private void Start()
    {
        _weapons = new IWeapon[_weaponSlots.Length];
        for (int i = 0; i < _weaponSlots.Length; i++)
            _weapons[i] = _weaponSlots[i] as IWeapon;

        ChangeWeapon(0);
    }


    //인풋
    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;

        if (Input.GetMouseButton(0))
            _current?.Attack();

        if (Input.GetKeyDown(KeyCode.R))
            _current?.Reload();

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
            ChangeWeapon((_index + (scroll > 0 ? 1 : -1) + _weapons.Length) % _weapons.Length);
    }

    private void ChangeWeapon(int idx)
    {
        _current?.Deselect();

        _index = idx;
        _current = _weapons[_index];

        _current?.Select();
    }


    //이경우 현재 무기의 현재 탄환 데이터가 필요
    ConsumableStat<int> _currentBullet;
    private void TryAttack()
    {

    }

    private void TryReload()
    {

    }
}
