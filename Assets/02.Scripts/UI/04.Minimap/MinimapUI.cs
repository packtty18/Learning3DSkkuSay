using ArtificeToolkit.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [Title("Reference")]
    [Required, SerializeField] private Image _backGroundImage;
    [Required, SerializeField] private Image _maskImage;

    [Title("Image Source")]
    [Required, SerializeField] private Sprite _maxSprite;
    [Required, SerializeField] private Sprite _minSprite;

    [Title("Size Set")]
    [SerializeField] private float _maxSize = 1000f;
    [SerializeField] private float _minSize = 320f;

    [Title("Position Set")]
    [SerializeField] private Vector2 _maxPosition = new Vector2(0, 0);
    [SerializeField] private Vector2 _minPosition = new Vector2(700, 300);

    private bool _isMaximized = false;

    private void Start()
    {
        ApplyMinimapState();
    }

    public void ChangeMode()
    {
        _isMaximized = !_isMaximized;
        ApplyMinimapState();
    }

    private void ApplyMinimapState()
    {
        if (_isMaximized)
        {
            _backGroundImage.sprite = _maxSprite;
            _maskImage.sprite = _maxSprite;
            SetRectTransform(_backGroundImage.rectTransform, _maxSize, _maxPosition);
        }
        else
        {
            _backGroundImage.sprite = _minSprite;
            _maskImage.sprite = _minSprite;
            SetRectTransform(_backGroundImage.rectTransform, _minSize, _minPosition);
        }
    }

    private void SetRectTransform(RectTransform rect, float size, Vector2 position)
    {
        rect.sizeDelta = new Vector2(size, size);
        rect.anchoredPosition = position;
    }
}
