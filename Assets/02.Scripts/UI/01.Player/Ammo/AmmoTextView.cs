using DG.Tweening;
using TMPro;
using UnityEngine;

public class AmmoTextView : MonoBehaviour
{
    [SerializeField] private TMP_Text _loadedAmmoText;
    [SerializeField] private TMP_Text _entireAmmoText;

    private Vector3 _loadedOrigin;
    private Vector3 _entireOrigin;

    private void Awake()
    {
        _loadedOrigin = _loadedAmmoText.rectTransform.localScale;
        _entireOrigin = _entireAmmoText.rectTransform.localScale;
    }

    public void SetLoadedAmmo(int value)
    {
        _loadedAmmoText.text = value.ToString();
        PlayPunch(_loadedAmmoText.rectTransform, _loadedOrigin, 0.2f);
    }

    public void SetEntireAmmo(int value)
    {
        _entireAmmoText.text = value.ToString();
        PlayPunch(_entireAmmoText.rectTransform, _entireOrigin, 0.15f);
    }

    private void PlayPunch(RectTransform rect, Vector3 origin, float scale)
    {
        rect.DOKill(true);

        rect.DOScale(origin + Vector3.one * scale, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                rect.DOScale(origin, 0.15f)
                    .SetEase(Ease.OutQuad);
            });
    }
}

