using UnityEngine;
using UnityEngine.AI;
using ArtificeToolkit.Attributes;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Title("Components")]
    [ReadOnly] private CharacterController _characterController;
    [ReadOnly] private AgentController _agent;

    [Required, SerializeField] private PlayerStat _stat;
    [Required, SerializeField] private Animator _animator;

    private IReadOnlyConsumable<float> _stamina => _stat.GetConsumable(EConsumableFloat.Stamina);
    private IReadOnlyValue<float> _moveSpeed => _stat.GetValue(EValueFloat.MoveSpeed);
    private IReadOnlyValue<float> _dashSpeed => _stat.GetValue(EValueFloat.DashSpeed);
    private IReadOnlyValue<float> _dashConsumeStamina => _stat.GetValue(EValueFloat.DashConsumeStaminaPerSecond);
    private IReadOnlyValue<float> _jumpPower => _stat.GetValue(EValueFloat.JumpPower);
    private IReadOnlyValue<float> _doubleJumpConsumeStamina => _stat.GetValue(EValueFloat.DoubleJumpConsumeStaminaPerOnce);
    private IReadOnlyValue<float> _staminaRegenDelay => _stat.GetValue(EValueFloat.StaminaRegenDelay);

    [Title("Runtime")]
    [ReadOnly] private float _yVelocity;
    [ReadOnly] private bool _canDoubleJump = true;
    [ReadOnly] private float _recoveryTimer = 0f;

    private CameraController _cameraController => CameraController.Instance;
    private bool _isDash => Input.GetKey(KeyCode.LeftShift);

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _agent = GetComponent<AgentController>();
    }

    private void Start()
    {
        _agent.SetAgent(_moveSpeed.Value, false);
    }

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;

        HandleGravity();

        switch (_cameraController.CurrentMode)
        {
            case CameraMode.FPS:
            case CameraMode.BackView:
                HandleFPSSimpleMove();
                break;

            case CameraMode.TPS:
                HandleTPSMove();
                break;
        }

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

    private void HandleFPSSimpleMove()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0f, v);
        _animator.SetFloat("Speed", direction.magnitude);
        _animator.SetBool("IsGround", _characterController.isGrounded);

        if (_cameraController.CurrentMode == CameraMode.FPS)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            direction = (camForward * v + camRight * h).normalized;
        }
        else
        {
            direction = transform.TransformDirection(direction);
            direction.y = 0f;
        }

        float speed = _moveSpeed.Value;

        if (_isDash && !_stamina.IsEmpty() && direction.sqrMagnitude > 0.01f)
        {
            speed = _dashSpeed.Value;
            ConsumeStamina(_dashConsumeStamina.Value * Time.deltaTime);
        }

        HandleJump();

        direction.y = _yVelocity;
        _characterController.Move(direction * speed * Time.deltaTime);
    }

    private void HandleTPSMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                _agent.SetAgentDestination(hit.point);
                Debug.Log($"[PlayerMove] TPS Move To {hit.point}");
            }
        }
    }

    private void HandleJump()
    {
        if (!Input.GetButtonDown("Jump"))
            return;

        if (_characterController.isGrounded)
        {
            _animator.SetTrigger("Jump");
            _yVelocity = _jumpPower.Value;
        }
        else if (_canDoubleJump && _stamina.Current >= _doubleJumpConsumeStamina.Value)
        {
            _yVelocity = _jumpPower.Value;
            _canDoubleJump = false;
            ConsumeStamina(_doubleJumpConsumeStamina.Value);
        }
    }


    private void HandleStaminaRecovery()
    {
        float dt = Time.deltaTime;

        if (_recoveryTimer > 0f)
        {
            _recoveryTimer -= dt;
            return;
        }

        if (!_stamina.IsFull())
        {
            _stamina.Regenerate(dt);
        }
    }

    private void ConsumeStamina(float amount)
    {
        _stamina.Consume(amount);
        _recoveryTimer = _staminaRegenDelay.Value;
    }
}