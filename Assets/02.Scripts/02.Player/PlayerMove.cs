using UnityEngine;
using UnityEngine.AI;
using ArtificeToolkit.Attributes;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Title("Components")]
    [ReadOnly, SerializeField] private CharacterController _characterController;
    [ReadOnly, SerializeField] private AgentController _agent;
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private Animator _animator;

    private MovementDataSO _data => _stat.MoveData;
    private ConsumableStat<float> _stamina => _stat.Stemina;

    private CameraController _cameraController => CameraController.Instance;
    private PlayerRotate _playerRotate => PlayerController.Instance.Rotate;

    [ReadOnly] private float _yVelocity;
    [ReadOnly] private bool _canDoubleJump = true;
    [ReadOnly] private float _recoveryTimer = 0f;

    private bool _isDash => Input.GetKey(KeyCode.LeftShift);

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _agent = GetComponent<AgentController>();

        //_agent.updateRotation = false; // NavMeshAgent 회전은 직접 처리
        //_agent.updateUpAxis = true;
    }

    private void Start()
    {
        _agent.SetAgent(_stat.MoveData.MoveSpeed, false);
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

    // 중력 처리
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
        else if (_cameraController.CurrentMode == CameraMode.BackView)
        {
            direction = transform.TransformDirection(direction);
            direction.y = 0f;
        }

        float speed = _data.MoveSpeed;
        if (_isDash && !_stamina.IsEmpty() && direction.sqrMagnitude > 0.01f)
        {
            speed = _data.DashSpeed;
            ConsumeStamina(_data.DashConsume * Time.deltaTime);
        }

        HandleJump();
        direction.y = _yVelocity;

        _characterController.Move(direction * speed * Time.deltaTime);
    }

    // TPS 이동 (NavMesh)
    private void HandleTPSMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                _agent.SetAgentDestination(hit.point);
                Debug.Log($"TPS 이동: {hit.point}");
            }
        }

        //Vector3 horizontalVelocity = new Vector3(_agent.velocity.x, 0, _agent.velocity.z);
        //if (horizontalVelocity.sqrMagnitude > 0.01f)
        //{
        //    _playerRotate.SetDirection(horizontalVelocity.normalized);
        //}
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (_characterController.isGrounded)
            {
                _animator.SetTrigger("Jump");
                _yVelocity = _data.JumpPower;
            }
            else if (_canDoubleJump && _stamina.Current >= _data.DoubleJumpConsume)
            {
                _yVelocity = _data.JumpPower;
                _canDoubleJump = false;
                ConsumeStamina(_data.DoubleJumpConsume);
            }
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
        _recoveryTimer = _data.RegenDelay;
    }
}
