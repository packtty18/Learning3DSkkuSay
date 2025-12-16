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
    private NavMeshAgent _agent;

    public TPSState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _rotationSpeed = speed;
        _agent = player.GetComponent<NavMeshAgent>();
    }

    public void Enter()
    {
        _agent.enabled = true;
        _playerRotate = _player.GetComponent<PlayerRotate>();

        _camera.DOMove(_pivot.position, 0.35f).SetEase(Ease.InOutSine);
        _camera.DORotate(_pivot.eulerAngles, 0.35f).SetEase(Ease.InOutSine);
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        float rotateSpeed = _rotationSpeed * Time.deltaTime;

        // 플레이어 회전
        _playerRotate.Rotate(mouseX * rotateSpeed);

        // 카메라 위치
        _camera.position = _pivot.position;

        // 이동 처리 (마우스 클릭 위치 이동)
        if (Input.GetMouseButtonDown(1))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit);
            _agent.SetDestination(hit.point);
            Debug.Log($"TPS 이동 : {hit.collider.name} , {hit.point}");
        }
    }

    public void Exit()
    {
        
        _agent.velocity = Vector3.zero;
        _agent.isStopped = true;
        _agent.ResetPath();
        _agent.enabled = false;
    }

    public Vector3 GetFireDirection(Transform firePos)
    {
        // 아직 자동타겟 사격 미구현
        return _player.forward;
    }
}
