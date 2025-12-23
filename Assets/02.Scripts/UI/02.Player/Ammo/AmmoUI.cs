using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private GunWeapon _gunWeapon;
    [SerializeField] private AmmoTextView _ammoTextView;
    [SerializeField] private ReloadUIView _reloadUIView;

    private void OnEnable()
    {
        _stat.InventoryBullet.Subscribe(_ammoTextView.SetEntireAmmo);
        _stat.LoadedBullet.Subscribe(_ammoTextView.SetLoadedAmmo);
        _gunWeapon.OnReloadTimerChange.Subscribe(OnReloadChanged);


        Sync();
    }

    private void OnDisable()
    {
        _stat.InventoryBullet.Unsubscribe(_ammoTextView.SetEntireAmmo) ;
        _stat.LoadedBullet.Unsubscribe(_ammoTextView.SetLoadedAmmo) ;
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
        _ammoTextView.SetLoadedAmmo(_stat.LoadedBullet.Current);
        _ammoTextView.SetEntireAmmo(_stat.InventoryBullet.Value);
        _reloadUIView.UpdateFill(
            _gunWeapon.ReloadTimer,
            _gunWeapon.ReloadTime);
    }
}
