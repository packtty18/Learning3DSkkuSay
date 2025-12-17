using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class TPSState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _rotationSpeed;

    private PlayerRotate _playerRotate;
    private AgentController _agent;

    public TPSState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _rotationSpeed = speed;
        _agent = player.GetComponent<AgentController>();
    }

    public void Enter()
    {
        _agent.SetEnable(true);
        _playerRotate = _player.GetComponent<PlayerRotate>();

        _camera.DOMove(_pivot.position, 0.35f).SetEase(Ease.InOutSine);
        _camera.DORotate(_pivot.eulerAngles, 0.35f).SetEase(Ease.InOutSine);
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        float rotateSpeed = _rotationSpeed * Time.deltaTime;

        // 플레이어 회전
        _playerRotate.Rotate(mouseX * rotateSpeed);

    }
    public void SetCameraPosition()
    {
        _camera.position = _pivot.position;
    }

    public void Exit()
    {
        _agent.AgentStop(true);
        _agent.SetEnable(false);
    }

    public Vector3 GetFireDirection(Transform firePos)
    {
        // 아직 자동타겟 사격 미구현
        return _player.forward;
    }
}
