using UnityEngine;
using UnityEngine.UI;

public class ReloadUIView : MonoBehaviour
{
    [SerializeField] private Image _fillImage;

    public void UpdateFill(float current, float max)
    {
        if (max <= 0f)
        {
            _fillImage.fillAmount = 0f;
            return;
        }

        _fillImage.fillAmount = Mathf.Clamp01(1f - current / max);
    }
}
