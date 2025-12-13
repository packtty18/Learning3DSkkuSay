using DG.Tweening;
using UnityEngine;
using static UnityEngine.InputSystem.Controls.AxisControl;

//상하는 카메라를 조작, 좌우및 위치는 피봇을 따라감
public class BackViewState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _speed;

    private PlayerRotate _playerRotate;

    private float tweenTime = 0.35f;

    private float _accumulateY;
    private const int Y_CLAMP = 15;
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
        FireRebound.OnRecoil += ApplyRecoil;

        _playerRotate = _player.GetComponent<PlayerRotate>();

        _camera.DOMove(_pivot.position, tweenTime).SetEase(Ease.InOutSine);
        _camera.DORotate(_pivot.eulerAngles, tweenTime)
            .SetEase(Ease.InOutSine);
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        float rotateSpeed = _speed * Time.deltaTime;
        _playerRotate.Rotate(mouseX* rotateSpeed);
        _accumulateY -= mouseY * rotateSpeed;
        _accumulateY = Mathf.Clamp(_accumulateY, -Y_CLAMP, Y_CLAMP);

        // 카메라 위치 변경
        _camera.position = _pivot.position;


        // 최종 회전: 마우스의 회전 + 반동 회전
        _camera.rotation = Quaternion.Euler(_accumulateY, _player.eulerAngles.y, 0f);
    }

    public void Exit()
    {
        FireRebound.OnRecoil -= ApplyRecoil;
    }

    private void ApplyRecoil(RecoilData data)
    {
        _accumulateY -= data.YStrength;
    }

    public Vector3 GetFireDirection(Transform firePos)
    {
        Ray ray = new Ray(_camera.position, _camera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            return (hit.point - firePos.position).normalized;
        }
        else
        {
            Vector3 farPoint = ray.GetPoint(200f);
            return (farPoint - firePos.position).normalized;
        }
    }
}
