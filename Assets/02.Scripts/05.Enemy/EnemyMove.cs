using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMove : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController _controller;

    [Header("Settings")]
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _gravity = -9.81f;

    private float _yVelocity = 0f;
    private Vector3 _moveDirection = Vector3.zero;

    private void Awake()
    {
        if (!_controller)
            _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity();

        //누적된 이동과 중력을 실행
        _controller.Move((_moveDirection + Vector3.up * _yVelocity) * Time.deltaTime);
        _moveDirection = Vector3.zero;
    }

    private void ApplyGravity()
    {
        if (!_controller.isGrounded)
        {
            _yVelocity += _gravity * Time.deltaTime;
        }
        else
        {
            _yVelocity = -1f; // grounded일 때 초기화
        }
    }

    //목적지 이동 용도
    public void MoveTo(Vector3 targetPos, float speed, bool isRotate = true)
    {
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0f;
        dir.Normalize();

        if (isRotate)
        {
            RotateTowards(dir);
        }
        _moveDirection += dir * speed;
    }

    //넉백등 단순 이동 용도
    public void MoveDirection(Vector3 direction, float speed, bool isRotate = false)
    {
        Vector3 dir = direction;
        dir.y = 0f;
        dir.Normalize();
        _moveDirection += dir * speed;

        if (isRotate)
        {
            RotateTowards(dir);
        }
    }

    private void RotateTowards(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
        }
    }
}
