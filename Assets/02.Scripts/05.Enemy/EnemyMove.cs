using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController _controller;

    [Header("Settings")]
    [SerializeField] private float _rotateSpeed = 10f;
    private float _yVelocity = 0f;

    private void Awake()
    {
        if (!_controller)
            _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity(); // 항상 중력 적용
    }

    // ⬇ 중력 처리
    private void ApplyGravity()
    {
        if (!_controller.isGrounded)
        {
            _yVelocity += Util.GRAVITY * Time.deltaTime;
        }

        Vector3 move = new Vector3(0, _yVelocity, 0);
        _controller.Move(move * Time.deltaTime);
    }

    // ⬇ 플레이어/목적지 방향으로 이동 + 회전
    public void MoveTo(Vector3 targetPos, float speed, bool isRotate = true)
    {
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0f;
        dir = dir.normalized;

        // 회전
        if (dir.sqrMagnitude > 0.01f && isRotate)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
        }

        OnMove(dir, speed);
    }

    // ⬇ 넉백 등 방향 기반 이동
    public void MoveDirection(Vector3 direction, float speed, bool isRotate = false)
    {
        Vector3 dir = direction;
        dir.y = 0f;
        dir = dir.normalized;

        // 넉백 시 회전은 선택적
        if (dir.sqrMagnitude > 0.01f && isRotate)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
        }

        OnMove(dir, speed);
    }

    private void OnMove(Vector3 dir, float speed)
    {
        Vector3 move = new Vector3(dir.x * speed, 0, dir.z * speed);
        _controller.Move(move * Time.deltaTime);
    }
}
