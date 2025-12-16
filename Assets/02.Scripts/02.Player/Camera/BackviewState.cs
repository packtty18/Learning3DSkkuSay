using DG.Tweening;
using UnityEngine;

public class BackViewState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _rotationSpeed;

    private PlayerRotate _playerRotate;
    private float _accumulateY;
    private const int Y_CLAMP = 15;



    private float tweenTime = 0.35f;

    public BackViewState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _rotationSpeed = speed;
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
        float rotateSpeed = _rotationSpeed * Time.deltaTime;

        // 좌우 회전은 플레이어
        _playerRotate.Rotate(mouseX * rotateSpeed);

        // 상하 회전은 카메라
        _camera.position = _pivot.position;
        _accumulateY -= mouseY * rotateSpeed;
        _accumulateY = Mathf.Clamp(_accumulateY, -Y_CLAMP, Y_CLAMP);

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