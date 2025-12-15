using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ArtificeToolkit.Attributes;

public class EnemyHealthbarUI : MonoBehaviour
{
    [Required,SerializeField] private EnemyHealth _enemy;
    [Required, SerializeField] private RectTransform _guageRect;
    [Required, SerializeField] private Image _frontGuage;
    [Required, SerializeField] private Image _backGuage;

    [Title("Timing")]
    [SerializeField] private float backDelay = 1f;
    [SerializeField] private float tweenDuration = 0.25f;

    [Title("Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 10f;
    [SerializeField] private int shakeVibrato = 10;

    [Title("Flash")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    private Tween _backTween;

    private Color _frontOriginalColor;
    private Color _backOriginalColor;

    private void Start()
    {
        _frontOriginalColor = _frontGuage.color;
        _backOriginalColor = _backGuage.color;

        _frontGuage.fillAmount = _enemy.Current / _enemy.MaxHealth;
        _backGuage.fillAmount = _frontGuage.fillAmount;
    }

    private void OnEnable()
    {
        _enemy.OnHealthChange += UpdateUI;
    }

    private void OnDisable()
    {
        _enemy.OnHealthChange -= UpdateUI;
    }

    private void UpdateUI()
    {
        float targetFill = _enemy.Current / _enemy.MaxHealth;

        _frontGuage.fillAmount = targetFill;

        _guageRect.DOKill();
        _guageRect.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato);

        FlashBar(_frontGuage);
        FlashBar(_backGuage);

        _backTween?.Kill();
        _backTween = DOVirtual.DelayedCall(backDelay, () =>
        {
            _backGuage.DOFillAmount(targetFill, tweenDuration).SetEase(Ease.OutQuad);
        });
    }

    private void FlashBar(Image bar)
    {
        bar.DOKill();
        Color original = (bar == _frontGuage) ? _frontOriginalColor : _backOriginalColor;

        bar.DOColor(flashColor, flashDuration)
           .SetLoops(2, LoopType.Yoyo)
           .OnComplete(() => bar.color = original);
    }
}
