using Unity.VisualScripting;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private GunWeapon _gunWeapon;
    [SerializeField] private AmmoTextView _ammoTextView;
    [SerializeField] private ReloadUIView _reloadUIView;

    private IReadOnlyConsumable<int> _invenBullet => _stat.GetConsumable(EConsumableInt.InvenBulletCount);
    private IReadOnlyConsumable<int> _loadBullet => _stat.GetConsumable(EConsumableInt.LoadedBulletCount);
    private void OnEnable()
    {
        if (_invenBullet == null || _loadBullet == null)
        {
            _stat.OnStatInitEnd.Subscribe(UIEnable);

            return;
        }

        UIEnable();
    }

    private void UIEnable()
    {
        _invenBullet.Subscribe(_ammoTextView.SetEntireAmmo);
        _loadBullet.Subscribe(_ammoTextView.SetLoadedAmmo);
        _gunWeapon.OnReloadTimerChange.Subscribe(OnReloadChanged);
        Sync();
    }

    private void OnDisable()
    {
        _invenBullet.Unsubscribe(_ammoTextView.SetEntireAmmo) ;
        _loadBullet.Unsubscribe(_ammoTextView.SetLoadedAmmo) ;
        _gunWeapon.OnReloadTimerChange.Unsubscribe(OnReloadChanged) ;
    }

    private void OnReloadChanged(float notUse)
    {
        _reloadUIView.UpdateFill(
            _gunWeapon.ReloadTimer,
            _gunWeapon.ReloadTime);
    }

    private void Sync()
    {
        _ammoTextView.SetLoadedAmmo(_loadBullet.Current);
        _ammoTextView.SetEntireAmmo(_invenBullet.Current);
        _reloadUIView.UpdateFill(
            _gunWeapon.ReloadTimer,
            _gunWeapon.ReloadTime);
    }
}
