using DG.Tweening;
using UnityEngine;
using static UnityEngine.InputSystem.Controls.AxisControl;

//상하회전은 카메라를 기준, 좌우는 피봇을 따라서
public class FPSState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _speed;

    private float _accumulateY;
    private float _tweenTime = 0.35f;

    private const int Y_CLAMP = 90;

    public FPSState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _speed = speed;
    }

    public void Enter()
    {
        Debug.Log("📷 Enter FPS Mode");
        _accumulateY = _camera.localEulerAngles.x;
        // 위치 이동
        _camera.DOMove(_pivot.position, _tweenTime)
            .SetEase(Ease.InOutSine);

        // 회전 이동
        _camera.DORotate(
            new Vector3(_accumulateY, _player.eulerAngles.y, 0f),
            _tweenTime
        )
        .SetEase(Ease.InOutSine);
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        float rotateSpeed = _speed * Time.deltaTime;

        // 상하 회전
        _accumulateY -= mouseY * rotateSpeed;
        _accumulateY = Mathf.Clamp(_accumulateY, -Y_CLAMP, Y_CLAMP);

        // 카메라 위치는 pivot 위치에 고정
        _camera.position = _pivot.position;

        // 피봇의 회전과 상하회전을 합쳐서 적용
        _camera.rotation = Quaternion.Euler(_accumulateY, _player.eulerAngles.y, 0f);
    }


    public void Exit() { }
}