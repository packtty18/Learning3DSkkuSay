using DG.Tweening;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private AmmoTextView _ammoTextView;
    [SerializeField] private ReloadUIView _reloadUIView;

    private void OnEnable()
    {
        _stat.InventoryBullet.OnValueChanged += _ammoTextView.SetEntireAmmo;

        _stat.LoadedBullet.OnCurrentChanged += _ammoTextView.SetLoadedAmmo;

        _stat.ReloadTimer.OnCurrentChanged += OnReloadChanged;
        _stat.ReloadTimer.OnMaxChanged += OnReloadChanged;

        Sync();
    }

    private void OnDisable()
    {
        _stat.InventoryBullet.OnValueChanged -= _ammoTextView.SetEntireAmmo;
        _stat.LoadedBullet.OnCurrentChanged -= _ammoTextView.SetLoadedAmmo;

        _stat.ReloadTimer.OnCurrentChanged -= OnReloadChanged;
        _stat.ReloadTimer.OnMaxChanged -= OnReloadChanged;
    }

    private void OnReloadChanged(float _)
    {
        _reloadUIView.UpdateFill(
            _stat.ReloadTimer.Current,
            _stat.ReloadTimer.Max);
    }

    private void Sync()
    {
        _ammoTextView.SetLoadedAmmo(_stat.LoadedBullet.Current);
        _ammoTextView.SetEntireAmmo(_stat.InventoryBullet.Value);
        _reloadUIView.UpdateFill(
            _stat.ReloadTimer.Current,
            _stat.ReloadTimer.Max);
    }
}
