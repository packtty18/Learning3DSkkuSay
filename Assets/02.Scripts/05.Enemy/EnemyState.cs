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
    [SerializeField] private Transform[] _patrolPoints;
    public Transform Player => _player;

    private EnemyMove _move;
    private EnemyAttack _attack;
    private EnemyHealth _health;
    private Vector3 _spawnPos;

    //추후 Enemystat으로 옮기고 도메인 추가하기
    [Header("Stat")]
    [SerializeField] private EEnemyState _state = EEnemyState.Idle;

    [Header("Idle")]
    private float _idleTimer = 0f;
    [SerializeField] private float _idleWaitTime = 3f; // Idle 후 Patrol 전환 시간

    [Header("Patrol")]
    private int _patrolIndex = 0;
    private Vector3 _lastTracePosition; // Trace 풀린 후 복귀 위치 저장

    [SerializeField] public float arrivalThreshold = 2f;
    [Header("Attack")]
    [SerializeField] private float _attackDamage = 5;
    [SerializeField] private float _attackSpeed = 2f;
    [SerializeField] private float _attackDistance = 1.2f;
    private float _attackTimer = 0f;

    [Header("Chase")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _detectDistance = 10f;

    [Header("Hit")]
    [SerializeField] private float _knockbackTime = 0.15f;

    public float DetectDistance => _detectDistance;
    public float AttackDistance => _attackDistance;

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
            _lastTracePosition = transform.position; // 추격 시 현재 위치 저장
            _state = EEnemyState.Trace;
            _idleTimer = 0f;
            return;
        }

        // Idle 3초 이상이면 Patrol 전환
        _idleTimer += Time.deltaTime;
        if (_patrolPoints.Length > 0 && _idleTimer >= _idleWaitTime)
        {
            DebugManager.Instance.Log("상태 전환: Idle → Patrol");
            _state = EEnemyState.Patrol;
            _idleTimer = 0f;
        }
    }

   
    private void Patrol()
    {
        if (_patrolPoints.Length == 0)
            return;

        Transform targetPoint = _patrolPoints[_patrolIndex];
        _move.MoveTo(targetPoint.position, _moveSpeed);

        float distance = Vector3.Distance(transform.position, targetPoint.position);
        if (distance < arrivalThreshold)
        {
            // 다음 패트롤 포인트로 이동
            _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
            DebugManager.Instance.Log($"패트롤 포인트 이동: {_patrolIndex}");
            _state = EEnemyState.Idle; // 이동 완료 후 Idle로 전환
        }

        // 플레이어 발견 시 Trace로 전환
        float playerDistance = Vector3.Distance(transform.position, _player.position);
        if (playerDistance <= _detectDistance)
        {
            DebugManager.Instance.Log("패트롤 → Trace");
            _lastTracePosition = transform.position; // 추격 후 복귀 위치 저장
            _state = EEnemyState.Trace;
        }
    }

    private void Comeback()
    {
        Vector3 targetPos = _spawnPos;

        // Trace 풀린 후 패트롤 이어가기
        if (_lastTracePosition != Vector3.zero && _patrolPoints.Length > 0)
        {
            targetPos = _lastTracePosition;
        }

        _move.MoveTo(targetPos, _moveSpeed);

        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist < 0.3f)
        {
            DebugManager.Instance.Log("상태 전환: Comeback → Idle");
            _state = EEnemyState.Idle;
            _lastTracePosition = Vector3.zero; // 복귀 완료
        }
    }


    private void Trace()
    {
        float distance = Vector3.Distance(transform.position, _player.position);

        _move.MoveTo(_player.position, _moveSpeed);

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
            
            //추후 : 공격 가능한 객체를 레이로 찾아서 IDamageable로 데미지 주기
            _attack.Attack(data);
        }
    }
    Coroutine hitRoutine = null;
    public void ApplyDamage(AttackData data)
    {
        if (_state == EEnemyState.Death)
            return;

        
        if (_health.ApplyDamage(data.Damage))
        {
            DebugManager.Instance.Log($"상태 전환 → Hit");
            _state = EEnemyState.Hit;

            if(hitRoutine != null)
            {
                StopCoroutine(hitRoutine);
            }

            hitRoutine = StartCoroutine(HitRoutine(data.HitDirection, data.Knockback.Power));
        }
        else
        {
            DebugManager.Instance.Log($"상태 전환 → Death");
            _state = EEnemyState.Death;
            StartCoroutine(DeathRoutine());
        }

        DebugManager.Instance.Log($"피격 발생 : {data.Attacker.name}가 {gameObject.name}에게 {data.Damage} 데미지. 남은체력 : {_health.Current} / {_health.MaxHealth}");
    }

    private IEnumerator HitRoutine(Vector3 direction, float knockbackForce)
    {
        float timer = 0f;
        Vector3 knockDir = direction.normalized;

        while (timer < _knockbackTime)
        {
            timer += Time.deltaTime;

            _move.MoveDirection(knockDir, knockbackForce);

            yield return null;
        }

        hitRoutine = null;
        _state = EEnemyState.Idle;
    }


    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2f);
        Util.ObjectDestroy(gameObject);
    }

    
}
