using DG.Tweening;
using UnityEngine;

//피봇의 위치만 따라감
public class TPSState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _speed;

    private float tweenTime = 0.35f;

    public TPSState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _speed = speed;
    }

    public void Enter()
    {
        Debug.Log("📷 Enter TPS Mode");

        _camera.DOMove(_pivot.position, tweenTime).SetEase(Ease.InOutSine);
        _camera.DORotate(_pivot.eulerAngles, tweenTime)
            .SetEase(Ease.InOutSine);
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        _camera.position = _pivot.position;
    }

    public void Exit() { }
}
