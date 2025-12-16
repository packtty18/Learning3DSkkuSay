using ArtificeToolkit.Attributes;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AI;
public enum ECameraMode
{
    FPS,
    TPS,
    BackView
}
public class CameraStateManager : MonoBehaviour
{
    public event Action OnStateChanged;

    [ReadOnly, SerializeField]
    private ICameraState _currentState;
    public ICameraState CurrentState => _currentState;

    public void ChangeState(ICameraState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
        OnStateChanged?.Invoke();
    }

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;

        if (_currentState == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _currentState.UpdateState(mouseX, mouseY);
    }
}


public class FPSState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _rotationSpeed;

    private PlayerRotate _playerRotate;
    private float _accumulateY;
    private const int Y_CLAMP = 90;

    public FPSState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _rotationSpeed = speed;
    }

    public void Enter()
    {
        _playerRotate = _player.GetComponent<PlayerRotate>();

        _camera.DOMove(_pivot.position, 0.35f).SetEase(Ease.InOutSine);
        _camera.DORotate(_pivot.eulerAngles, 0.35f).SetEase(Ease.InOutSine);

        _accumulateY = _camera.localEulerAngles.x;
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        float rotateSpeed = _rotationSpeed * Time.deltaTime;

        // 좌우는 플레이어
        _playerRotate.Rotate(mouseX * rotateSpeed);

        // 상하는 카메라
        _camera.position = _pivot.position;
        _accumulateY -= mouseY * rotateSpeed;
        _accumulateY = Mathf.Clamp(_accumulateY, -Y_CLAMP, Y_CLAMP);

        _camera.rotation = Quaternion.Euler(_accumulateY, _player.eulerAngles.y, 0f);
    }

    public void Exit() { }

    public Vector3 GetFireDirection(Transform firePos)
    {
        return _camera.forward;
    }
}

public class BackViewState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _rotationSpeed;

    private PlayerRotate _playerRotate;
    private float _accumulateY;
    private const int Y_CLAMP = 15;

    public BackViewState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _rotationSpeed = speed;
    }

    public void Enter()
    {
        _playerRotate = _player.GetComponent<PlayerRotate>();

        _camera.DOMove(_pivot.position, 0.35f).SetEase(Ease.InOutSine);
        _camera.DORotate(_pivot.eulerAngles, 0.35f).SetEase(Ease.InOutSine);

        _accumulateY = _camera.localEulerAngles.x;
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

    public void Exit() { }

    public Vector3 GetFireDirection(Transform firePos)
    {
        return _camera.forward;
    }
}

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

    public void Exit() { }

    public Vector3 GetFireDirection(Transform firePos)
    {
        // 아직 자동타겟 사격 미구현
        return _player.forward;
    }
}
