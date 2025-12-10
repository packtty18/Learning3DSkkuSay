using DG.Tweening;
using UnityEngine;

//상하는 카메라를 조작, 좌우및 위치는 피봇을 따라감
public class BackViewState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _speed;

    private float tweenTime = 0.35f;

    public BackViewState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _speed = speed;
    }

    public void Enter()
    {
        DebugManager.Instance.Log("Enter BackView Mode");

        _camera.DOMove(_pivot.position, tweenTime).SetEase(Ease.InOutSine);
        _camera.DORotate(_pivot.eulerAngles, tweenTime)
            .SetEase(Ease.InOutSine);
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        _camera.position = _pivot.position;
        _camera.rotation = _pivot.rotation;
    }

    public void Exit() 
    { 
    }
}
