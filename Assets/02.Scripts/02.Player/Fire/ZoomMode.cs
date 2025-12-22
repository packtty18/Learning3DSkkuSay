using DG.Tweening;
using System;
using UnityEngine;

public enum EZoomMode
{ 
    ZoomOut,
    ZoomIn
}

public class WeaponZoom : MonoBehaviour
{
    public EZoomMode ZoomMode = EZoomMode.ZoomOut;

    public Camera Camera;
    public GameObject Normal;
    public GameObject Zoom;

    private Tween _tween;

    private void Awake()
    {
        ActiveCrossHair(false);
        Camera = Camera.main;
    }

    public void ChangeZoom()
    {
        if(ZoomMode == EZoomMode.ZoomOut)
        {
            ZoomMode = EZoomMode.ZoomIn;
            Camera.fieldOfView = 10;
            _tween?.Kill();
            _tween = Camera.DOFieldOfView(10, 0.5f);
            ActiveCrossHair(true);
        }
         else
        {
            ZoomMode = EZoomMode.ZoomOut;
            _tween?.Kill();
            _tween = Camera.DOFieldOfView(60, 1f);
            ActiveCrossHair(false);
        }
    }


    public void ActiveCrossHair(bool isZoom)
    {
        Normal.SetActive(!isZoom);
        Zoom.SetActive(isZoom);
    }

}
