using DG.Tweening;
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

    private ConsumableStat<int> _loadedAmmo => _stat.LoadedBulletCount;
    private ValueStat<int> _entireAmmo => _stat.EntireBullet;
    private ConsumableStat<float> _reloadTime => _stat.ReloadTime;

    // Original & Target scales
    private Vector3 _currentOriginalScale;
    private Vector3 _entireOriginalScale;

    private Vector3 _currentTargetScale;
    private Vector3 _entireTargetScale;

    private void Awake()
    {
        // Save initial scales
        _currentOriginalScale = _currentAmmoText.rectTransform.localScale;
        _entireOriginalScale = _entireAmmoText.rectTransform.localScale;

        // Define target punch scale
        _currentTargetScale = _currentOriginalScale + Vector3.one * 0.2f;
        _entireTargetScale = _entireOriginalScale + Vector3.one * 0.15f;
    }

    private void OnEnable()
    {
        _loadedAmmo.ValueChanged += UpdateLoadAmmoText;
        _entireAmmo.ValueChanged += UpdateEntireAmmoText;
        _reloadTime.ValueChanged += UpdateReloadFill;
    }

    private void OnDisable()
    {
        _loadedAmmo.ValueChanged -= UpdateLoadAmmoText;
        _entireAmmo.ValueChanged -= UpdateEntireAmmoText;
        _reloadTime.ValueChanged -= UpdateReloadFill;
    }

    private void Start()
    {
        UpdateLoadAmmoText();
        UpdateEntireAmmoText();
        UpdateReloadFill();
    }

    private void UpdateLoadAmmoText()
    {
        _currentAmmoText.text = _loadedAmmo.CurrentValue.ToString();
        PlayPunch(_currentAmmoText.rectTransform, _currentOriginalScale, _currentTargetScale);
    }

    private void UpdateEntireAmmoText()
    {
        _entireAmmoText.text = _entireAmmo.Value.ToString();
        PlayPunch(_entireAmmoText.rectTransform, _entireOriginalScale, _entireTargetScale);
    }

    /// <summary>
    /// 안전한 Punch 효과 (중간에 끊겨도 scale 누적 없음)
    /// </summary>
    private void PlayPunch(RectTransform rect, Vector3 original, Vector3 target)
    {
        rect.DOKill(true); // kill & complete

        // Move to target
        rect.DOScale(target, 0.1f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Return to original
                rect.DOScale(original, 0.15f).SetEase(Ease.OutQuad);
            });
    }

    /// <summary> Reload gauge update </summary>
    private void UpdateReloadFill()
    {
        float ratio = 1f - (_reloadTime.CurrentValue / _reloadTime.MaxValue);
        ratio = Mathf.Clamp01(ratio);
    }
}
