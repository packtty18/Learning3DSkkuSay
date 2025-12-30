using DG.Tweening;
using System.Text;
using TMPro;
using UnityEngine;

public class AmmoTextView : MonoBehaviour
{
    [SerializeField] private TMP_Text _loadedAmmoText;
    [SerializeField] private TMP_Text _entireAmmoText;

    private StringBuilder _sb;
    public void SetLoadedAmmo(int value)
    {
        //value.ToString이 가비지를 생성하는가? => ToString()으로의 형변환 또한 string을 새로 생성하기에 힙에 가비지가 발생한다.
        //여기에 멤버로 스트링빌더를 생성하고 값을 주는건 어떤가
        _loadedAmmoText.SetText(value.ToString());
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

