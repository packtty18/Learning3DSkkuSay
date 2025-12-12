using System;
using System.Collections;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public enum EEnemyState
{
    Idle,
    Trace,
    Comeback,
    Attack,
    Hit,
    Death,
    Patrol,
}

public class EnemyState : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private Transform _player;
    public Transform Player => _player;

    private EnemyMove _move;
    private EnemyAttack _attack;
    private EnemyHealth _health;

    [Header("Config")]
    [SerializeField] private float _detectDistance = 10f;
    [SerializeField] private float _attackDistance = 1.2f;
    [SerializeField] private float _attackDamage = 5;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _attackSpeed = 2f;

    [SerializeField] private float _knockbackTime = 0.15f;

    public float DetectDistance => _detectDistance;
    public float AttackDistance => _attackDistance;

    [SerializeField] private EEnemyState _state = EEnemyState.Idle;
    private float _attackTimer = 0f;
    private Vector3 _spawnPos;

    private void Start()
    {
        _move = GetComponent<EnemyMove>();
        _attack = GetComponent<EnemyAttack>();
        _health = GetComponent<EnemyHealth>();
        _attackTimer = 0;
        _spawnPos = transform.position;
    }

    private void Update()
    {
        switch (_state)
        {
            case EEnemyState.Idle:
                Idle();
                break;
            case EEnemyState.Trace:
                Trace();
                break;
            case EEnemyState.Patrol:
                Patrol();
                break;
            case EEnemyState.Comeback:
                Comeback();
                break;
            case EEnemyState.Attack:
                Attack();
                break;
        }
    }

    

    private void Idle()
    {
        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance <= _detectDistance)
        {
            DebugManager.Instance.Log("상태 전환: Idle → Trace");
            _state = EEnemyState.Trace;
        }
    }

    private void Trace()
    {
        float distance = Vector3.Distance(transform.position, _player.position);

        _move.MoveWithRotation(_player.position, _moveSpeed);

        if (distance <= _attackDistance)
        {
            DebugManager.Instance.Log("상태 전환: Trace → Attack");
            _state = EEnemyState.Attack;
        }

        if (distance > _detectDistance * 1.2f) // 살짝 여유값 20% 줘서 튀는거 방지
        {
            DebugManager.Instance.Log("상태 전환: Trace → Comeback");
            _state = EEnemyState.Comeback;
            return;
        }
    }

    private void Patrol()
    {

    }


    private void Comeback()
    {
        _move.MoveWithRotation(_spawnPos, _moveSpeed);

        float dist = Vector3.Distance(transform.position, _spawnPos);
        if (dist < 0.3f)
        {
            DebugManager.Instance.Log("상태 전환: Comeback → Idle");
            _state = EEnemyState.Idle;
        }
    }

    private void Attack()
    {
        float distance = Vector3.Distance(transform.position, _player.position);

        //공격 범위를 벗어난다면?
        if (distance > _attackDistance)
        {
            DebugManager.Instance.Log("상태 전환: Attack → Trace");
            _state = EEnemyState.Trace;
            _attackTimer = 0;
            return;
        }
       
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackSpeed)
        {
            _attackTimer = 0f;

            AttackData data = new AttackData();
            data.Damage = _attackDamage;
            data.HitDirection = transform.forward;
            data.Attacker = gameObject;
            
            _attack.Attack(data);
        }
    }

    public void ApplyDamage(AttackData data)
    {
        if (_state == EEnemyState.Hit || _state == EEnemyState.Death)
            return;

        
        if (_health.ApplyDamage(data.Damage))
        {
            DebugManager.Instance.Log($"상태 전환 → Hit");
            _state = EEnemyState.Hit;
            StartCoroutine(HitRoutine(data.HitDirection, data.KnockbackPower));
        }
        else
        {
            DebugManager.Instance.Log($"상태 전환 → Death");
            _state = EEnemyState.Death;
            StartCoroutine(DeathRoutine());
        }
    }

    private IEnumerator HitRoutine(Vector3 direction, float knockbackForce)
    {
        float timer = 0f;
        Vector3 knockDir = direction.normalized;

        while (timer < _knockbackTime)
        {
            timer += Time.deltaTime;

            // CharacterController 넉백 이동
            _move.Knockback(knockDir, knockbackForce);

            yield return null;
        }
        _state = EEnemyState.Idle;
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    
}
