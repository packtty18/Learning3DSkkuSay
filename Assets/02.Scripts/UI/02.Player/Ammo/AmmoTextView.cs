using DG.Tweening;
using TMPro;
using UnityEngine;

public class AmmoTextView : MonoBehaviour
{
    [SerializeField] private TMP_Text _loadedAmmoText;
    [SerializeField] private TMP_Text _entireAmmoText;


    public void SetLoadedAmmo(int value)
    {
        _loadedAmmoText.text = value.ToString();
        PlayPunch(_loadedAmmoText.rectTransform, 0.2f);
    }

    public void SetEntireAmmo(int value)
    {
        _entireAmmoText.text = value.ToString();
        PlayPunch(_entireAmmoText.rectTransform, 0.15f);
    }

    private void PlayPunch(RectTransform rect, float scale)
    {
        rect.DOKill(true);

        rect.DOScale(Vector3.one * scale, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                rect.DOScale(Vector3.one, 0.15f)
                    .SetEase(Ease.OutQuad);
            });
    }
}

