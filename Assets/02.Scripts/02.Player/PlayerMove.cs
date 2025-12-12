using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerStat _stat;

    private ValueStat<float> _staminaRegenDelay =>_stat.SteminaRegenDelay;
    private ValueStat<float> _dashConsume => _stat.DashConsume;
    private ValueStat<float> _doubleJumpCost => _stat.DoubleJumpConsume;

    private ValueStat<float> _moveSpeed => _stat.MoveSpeed;
    private ValueStat<float> _dashSpeed => _stat.DashSpeed;
    private ValueStat<float> _jumpPower => _stat.JumpPower;
    private ConsumableStat<float> _stemina => _stat.Stemina;

    private bool _isDash => Input.GetKey(KeyCode.LeftShift);

    private float _yVelocity;
    private bool _canDoubleJump = true;
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

    private void HandleGravity()
    {
        if (_characterController.isGrounded)
        {
            _yVelocity = -1f;
            _canDoubleJump = true; 
        }
        else
        {
            _yVelocity += Util.GRAVITY * Time.deltaTime;
        }
    }
    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(h, 0f, v).normalized;

        if (_cameraController.CurrentMode == CameraMode.FPS)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            direction = (camForward * v + camRight * h).normalized;
        }
        else if (_cameraController.CurrentMode == CameraMode.TPS)
        {
            direction = transform.TransformDirection(direction);
            direction.y = 0f;
        }
        else if (_cameraController.CurrentMode == CameraMode.BackView)
        {
            direction = transform.TransformDirection(direction);
            direction.y = 0f;
        }

        direction.y = 0f;

        float currentSpeed = _moveSpeed.Value;

        if (_isDash && !_stemina.IsEmpty() && direction.sqrMagnitude > 0.1f)
        {
            currentSpeed = _dashSpeed.Value;
            ConsumeStamina(_dashConsume.Value * Time.deltaTime);
        }

        HandleJump();
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
                _yVelocity = _jumpPower.Value;
                DebugManager.Instance.Log("Jump!");
            }
            else if (_canDoubleJump && _stemina.CurrentValue >= _doubleJumpCost.Value)
            {
                _yVelocity = _jumpPower.Value;
                _canDoubleJump = false;
                ConsumeStamina(_doubleJumpCost.Value);
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

        if (!_stemina.IsFull())
        {
            _stemina.Regenerate();
        }
    }

    // 스태미나 소비 처리
    private void ConsumeStamina(float amount)
    {
        _stemina.Consume(amount);
        _recoveryTimer = _staminaRegenDelay.Value; 
    }
}
