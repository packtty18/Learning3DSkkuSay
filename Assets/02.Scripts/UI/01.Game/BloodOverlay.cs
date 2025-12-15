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

    [SerializeField]
    private float fadeDuration = 0.5f;

    private void Start()
    {
        float alpha = 1f - (_health.Current / _health.Max);
        SetAlpha(alpha);
    }

    private void OnEnable()
    {
        _health.OnCurrentChanged += ChangedWithEffect;
        _health.OnMaxChanged += ChangedWithoutEffect;
    }

    private void OnDisable()
    {
        _health.OnCurrentChanged -= ChangedWithEffect;
        _health.OnMaxChanged -= ChangedWithoutEffect;
    }

    private void ChangedWithEffect(float notUse)
    {
        float targetAlpha = 1f;
        SetAlpha(targetAlpha);
        ChangedWithoutEffect(notUse);
    }

    private void ChangedWithoutEffect(float notUse)
    {
        float finalAlpha = 1f - (_health.Current / _health.Max);

        OverlayUI.DOKill();
        OverlayUI.DOFade(finalAlpha, fadeDuration).SetEase(Ease.OutQuad);
    }

    private void SetAlpha(float alpha)
    {
        Color color = OverlayUI.color;
        color.a = alpha;
        OverlayUI.color = color;
    }
}
