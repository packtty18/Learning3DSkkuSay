using ArtificeToolkit.Attributes;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Title("Components")]
    [ReadOnly, SerializeField] private CharacterController _characterController;
    [ReadOnly, SerializeField] private PlayerStat _stat;

    private MovementDataSO _data => _stat.MoveData;
    private ConsumableStat<float> _stemina => _stat.Stemina;

    [Title("Runtime State")]
    [ReadOnly] private float _yVelocity;
    [ReadOnly] private bool _canDoubleJump = true;
    [ReadOnly] private float _recoveryTimer = 0f;

    private CameraController _cameraController => CameraController.Instance;
    private bool _isDash => Input.GetKey(KeyCode.LeftShift);




    public void Init()
    {
        _characterController = GetComponent<CharacterController>();

        if (!PlayerController.IsExist())
        {
            DebugManager.Instance.Log("PlayerController is not Setted");
            return;
        }

        _stat = PlayerController.Instance.Stat;
    }

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;
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

        float currentSpeed = _data.MoveSpeed;

        if (_isDash && !_stemina.IsEmpty() && direction.sqrMagnitude > 0.1f)
        {
            currentSpeed = _data.DashSpeed;
            ConsumeStamina(_data.DashConsume * Time.deltaTime);
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
                _yVelocity = _data.JumpPower;
                DebugManager.Instance.Log("Jump!");
            }
            else if (_canDoubleJump && _stemina.Current >= _data.DoubleJumpConsume)
            {
                _yVelocity = _data.JumpPower;
                _canDoubleJump = false;
                ConsumeStamina(_data.DoubleJumpConsume);
                DebugManager.Instance.Log($"Double Jump (Stamina - {_data.DoubleJumpConsume})!");
            }
        }
    }

    // 스태미나 회복
    private void HandleStaminaRecovery()
    {
        //해당 시간이 지나야 회복 시작
        float delta = Time.deltaTime;   
        if (_recoveryTimer > 0f)
        {
            _recoveryTimer -= delta;
            return;
        }

        if (!_stemina.IsFull())
        {
            _stemina.Regenerate(delta);
        }
    }

    // 스태미나 소비 처리
    private void ConsumeStamina(float amount)
    {
        _stemina.Consume(amount);
        _recoveryTimer = _data.RegenDelay; 
    }
}
