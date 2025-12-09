using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private const float GRAVITY = -9.81f;

    [Header("Components")]
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private CharacterController _characterController;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _dashSpeed = 10f;
    [SerializeField] private float _jumpPower = 5f;

    [Header("Stamina Settings")]
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _currentStamina = 100f;
    [SerializeField] private float _staminaRecoveryRate = 10f; // 초당 회복
    [SerializeField] private float _staminaRecoveryDelay = 2f; // 회복 딜레이
    [SerializeField] private float _staminaDashCostPerSec = 2f; // 달릴 때 초당 소비
    [SerializeField] private float _doubleJumpCost = 20f; // 이단점프 시 소비 => 스태미나 모션 기준

    public float MaxStamina => _maxStamina;
    public float CurrentStamina => _currentStamina;
    public float DoubleJumpCost => _doubleJumpCost;

    private float _yVelocity;
    private bool _canDoubleJump = true;
    private bool _isDash => Input.GetKey(KeyCode.LeftShift);

    private float _recoveryTimer = 0f;

    private void Awake()
    {
        if (!TryGetComponent(out _characterController))
        {
            _characterController = gameObject.AddComponent<CharacterController>();
        }
    }

    private void Update()
    {
        HandleGravity();
        HandleMovement();
        
        HandleStaminaRecovery();
    }

    // ⬇ 중력 처리
    private void HandleGravity()
    {
        if (_characterController.isGrounded)
        {
            _yVelocity = -1f;
            _canDoubleJump = true; // 착지 시 이단점프 회복
        }
        else
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }
    }

    // ⬇ 이동 + 대시 스태미나 소모
    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(h, 0f, v).normalized;

        if (_cameraController.CurrentMode == CameraMode.FPS)
        {
            // FPS는 카메라 바라보는 방향 기준 이동
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            direction = (camForward * v + camRight * h).normalized;
        }
        else if (_cameraController.CurrentMode == CameraMode.TPS)
        {
            // TPS는 플레이어 바라보는 방향 기준 이동
            direction = transform.TransformDirection(direction);
            direction.y = 0f;
        }
        else if (_cameraController.CurrentMode == CameraMode.BackView)
        {
            // BackView는 기본적으로 플레이어 forward 기준 이동
            direction = transform.TransformDirection(direction);
            direction.y = 0f;
        }

        direction.y = 0f;

        float currentSpeed = _moveSpeed;

        // 대쉬 적용
        if (_isDash && _currentStamina > 0f && direction.sqrMagnitude > 0.1f)
        {
            currentSpeed = _dashSpeed;
            ConsumeStamina(_staminaDashCostPerSec * Time.deltaTime);
        }

        //점프실행
        HandleJump();

        // 중력 적용
        direction.y = _yVelocity;

        _characterController.Move(direction * currentSpeed * Time.deltaTime);
    }

    //점프 로직 분리
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (_characterController.isGrounded)
            {
                _yVelocity = _jumpPower;
                DebugManager.Instance.Log("Jump!");
            }
            else if (_canDoubleJump && _currentStamina >= _doubleJumpCost)
            {
                _yVelocity = _jumpPower;
                _canDoubleJump = false;
                ConsumeStamina(_doubleJumpCost);
                DebugManager.Instance.Log("Double Jump (Stamina -20)!");
            }
        }
    }

    // 스태미나 회복
    private void HandleStaminaRecovery()
    {
        //해당 시간이 지나야 회복 시작
        if (_recoveryTimer > 0f)
        {
            _recoveryTimer -= Time.deltaTime;
            return;
        }

        if (_currentStamina < _maxStamina)
        {
            _currentStamina += _staminaRecoveryRate * Time.deltaTime;
            _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
        }
    }

    // 스태미나 소비 처리
    private void ConsumeStamina(float amount)
    {
        _currentStamina -= amount;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
        _recoveryTimer = _staminaRecoveryDelay; 
    }
}
