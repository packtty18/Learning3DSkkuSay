using DG.Tweening;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image _reloadFillImage;
    [SerializeField] private TMP_Text _currentAmmoText;
    [SerializeField] private TMP_Text _entireAmmoText;

    [Header("Player Reference")]
    [SerializeField] private PlayerStat _stat;
    private ValueStat<int> _invenAmmo => _stat.InventoryBullet;
    private ConsumableStat<int> _loadedAmmo => _stat.LoadedBullet;
    private ConsumableStat<float> _reloadTimer => _stat.ReloadTimer;

    // Scales
    private Vector3 _currentOriginalScale;
    private Vector3 _entireOriginalScale;

    private Vector3 _currentTargetScale;
    private Vector3 _entireTargetScale;

    private void Awake()
    {
        _currentOriginalScale = _currentAmmoText.rectTransform.localScale;
        _entireOriginalScale = _entireAmmoText.rectTransform.localScale;

        _currentTargetScale = _currentOriginalScale + Vector3.one * 0.2f;
        _entireTargetScale = _entireOriginalScale + Vector3.one * 0.15f;
    }

    private void OnEnable()
    {
        if (_stat == null)
            return;

        _invenAmmo.OnValueChanged += OnEntireAmmoChanged;

        _loadedAmmo.OnCurrentChanged += OnLoadedAmmoChanged;
        _loadedAmmo.OnMaxChanged += OnLoadedAmmoMaxChanged;

        _reloadTimer.OnCurrentChanged += OnReloadTimeChanged;
        _reloadTimer.OnMaxChanged += OnReloadTimeMaxChanged;

        // 즉시 동기화
        SyncAll();
    }

    private void OnDisable()
    {
        if (_stat == null)
            return;

        _invenAmmo.OnValueChanged -= OnEntireAmmoChanged;

        _loadedAmmo.OnCurrentChanged -= OnLoadedAmmoChanged;
        _loadedAmmo.OnMaxChanged -= OnLoadedAmmoMaxChanged;

        _reloadTimer.OnCurrentChanged -= OnReloadTimeChanged;
        _reloadTimer.OnMaxChanged -= OnReloadTimeMaxChanged;
    }

    private void SyncAll()
    {
        OnLoadedAmmoChanged(_loadedAmmo.Current);
        OnEntireAmmoChanged(_invenAmmo.Value);
        UpdateReloadFill(_reloadTimer.Current, _reloadTimer.Max);
    }

    #region Ammo Text

    private void OnLoadedAmmoChanged(int current)
    {
        _currentAmmoText.text = current.ToString();
        PlayPunch(_currentAmmoText.rectTransform, _currentOriginalScale, _currentTargetScale);
    }

    // Max 변경 시에도 숫자 갱신 필요하면 사용
    private void OnLoadedAmmoMaxChanged(int max)
    {
        // 현재는 UI 변화 없음 (확장 포인트)
    }

    private void OnEntireAmmoChanged(int current)
    {
        _entireAmmoText.text = current.ToString();
        PlayPunch(_entireAmmoText.rectTransform, _entireOriginalScale, _entireTargetScale);
    }

    #endregion

    #region Reload Fill

    private void OnReloadTimeChanged(float current)
    {
        UpdateReloadFill(current, _reloadTimer.Max);
    }

    private void OnReloadTimeMaxChanged(float max)
    {
        UpdateReloadFill(_reloadTimer.Current, max);
    }

    private void UpdateReloadFill(float current, float max)
    {
        if (max <= 0f)
        {
            _reloadFillImage.fillAmount = 0f;
            return;
        }

        float ratio = 1f - (current / max);
        _reloadFillImage.fillAmount = Mathf.Clamp01(ratio);
    }

    #endregion

    private void PlayPunch(RectTransform rect, Vector3 original, Vector3 target)
    {
        rect.DOKill(true);

        rect.DOScale(target, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                rect.DOScale(original, 0.15f)
                    .SetEase(Ease.OutQuad);
            });
    }
}
