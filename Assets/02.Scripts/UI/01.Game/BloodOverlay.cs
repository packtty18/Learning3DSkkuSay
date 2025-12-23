using ArtificeToolkit.Attributes;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BloodOverlay : MonoBehaviour
{
    [Title("Components")]
    [Required, SerializeField]
    private Image OverlayUI;

    [Required, SerializeField]
    private PlayerStat _stat;
    private IReadOnlyConsumable<float> _health => _stat.GetConsumable(EConsumableFloat.Health);

    [Title("Settings")]
    [SerializeField]
    private float fadeDuration = 0.5f;


    private void OnEnable()
    {
        if(_health == null)
        {
            _stat.OnStatInitEnd.Subscribe(UIEnable);
            return;
        }

        UIEnable();
    }

    private void UIEnable()
    {
        _health.Subscribe(Sync);
        SetAlpha(GetNextAlpha());
    }

    private void OnDisable()
    {
        _health.Unsubscribe(Sync);
    }

    private void Sync(float notUse)
    {
        float alpha = GetNextAlpha();
        SetAlpha(alpha);
        OverlayUI.DOKill();
        OverlayUI
            .DOFade(alpha, fadeDuration)
            .SetEase(Ease.OutQuad);
    }

    private float GetNextAlpha()
    {
        return 1f - (_health.Current / _health.Max);
    }

    private void SetAlpha(float alpha)
    {
        Color color = OverlayUI.color;
        color.a = alpha;
        OverlayUI.color = color;
    }
}