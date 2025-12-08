using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private const float GRAVITY = -9.81f;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _DashSpeed = 10;
    [SerializeField] private float _jumpPower = 5f;

    [SerializeField] private float _stamina = 100;
    [SerializeField] private float _staminaRecoveryRatePerSecond = 1;
    [SerializeField] private float _staminaDelayTime = 3;

    
    private float _yVelocity;
    private bool _isDash => Input.GetKey(KeyCode.LeftShift);
    private void Awake()
    {
        if(TryGetComponent(out CharacterController characterController))
        {
            _characterController = characterController;
        }
        else
        {
            transform.AddComponent<CharacterController>();
            _characterController = transform.GetComponent<CharacterController>();
        }
    }
    private void Update()
    {
        _yVelocity += GRAVITY * Time.deltaTime;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(h, 0, v).normalized;
        direction = Camera.main.transform.TransformDirection(direction);

        if(Input.GetButtonDown("Jump") && _characterController.isGrounded)
        {
            _yVelocity = _jumpPower;
        }
        direction.y = _yVelocity;


        float MoveSpeed = _isDash ? _DashSpeed : _moveSpeed;
        _characterController.Move(direction * _moveSpeed * Time.deltaTime);
    }
}
