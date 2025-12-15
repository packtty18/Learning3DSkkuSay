using ArtificeToolkit.Attributes;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BloodOverlay : MonoBehaviour
{
    [Required, SerializeField]
    private Image OverlayUI;
    [Required, SerializeField] 
    private PlayerStat _stat;
    private ConsumableStat<float> _health => _stat.Health;

    [SerializeField, Tooltip("얼마나 빨리 알파가 줄어드는지")]
    private float fadeDuration = 0.5f;

    private void Start()
    {
        UpdateOverlayImmediate();
    }

    private void OnEnable()
    {
        _health.OnCurrentChanged += OnHealthChanged;
        _health.OnMaxChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _health.OnCurrentChanged -= OnHealthChanged;
        _health.OnMaxChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float notUse)
    {
        float targetAlpha = 1f;
        SetOverlayAlpha(targetAlpha);

        float finalAlpha = 1f - (_health.Current / _health.Max);

        OverlayUI.DOKill();
        OverlayUI.DOFade(finalAlpha, fadeDuration).SetEase(Ease.OutQuad);
    }

    private void UpdateOverlayImmediate()
    {
        float alpha = 1f - (_health.Current / _health.Max);
        SetOverlayAlpha(alpha);
    }

    private void SetOverlayAlpha(float alpha)
    {
        Color color = OverlayUI.color;
        color.a = alpha;
        OverlayUI.color = color;
    }
}
