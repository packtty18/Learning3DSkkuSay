using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine;

public class MinimapUI : MonoBehaviour
{
    [Title("Reference")]
    [SerializeField] private Camera _minimapCamera;

    [Title("Size Settings")]
    [SerializeField] private float _maxInSize = 2f;    // 축소
    [SerializeField] private float _maxOutSize = 10f;  // 확대
    [SerializeField] private float _amount = 1f;
    [SerializeField] private float _tweenDuration = 1f;

    private Tween _tween;
    public void ZoomIn()
    {
        float target = _minimapCamera.orthographicSize - _amount;
        target = Mathf.Clamp(target, _maxInSize, _maxOutSize);
        _tween?.Kill();
        _tween = _minimapCamera.DOOrthoSize(target, _tweenDuration).SetEase(Ease.OutCubic);
    }

    public void ZoomOut()
    {
        float target = _minimapCamera.orthographicSize + _amount;
        target = Mathf.Clamp(target, _maxInSize, _maxOutSize);
        _tween?.Kill();
        _tween = _minimapCamera.DOOrthoSize(target, _tweenDuration).SetEase(Ease.OutCubic);
    }
}
