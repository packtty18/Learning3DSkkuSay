using ArtificeToolkit.Attributes;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Title("Core References")]
    public PlayerController Player;

    
    [Title("Enemy Components")]

    [Required]public EnemyStat Stat;
    [Required]public EnemyMove Move;
    [Required]public EnemyAttack Attack;
    [Required]public EnemyDetector Detector;
    [Required]public Animator Animator;
    [Required]public AgentController Agent;

    [Title("UI")]
    [Required]public GameObject EnemyUI;
    [ReadOnly]private EnemyStateMachine _stateMachine;


    #region 디버그
    public event Action<EnemyController> OnReturnedToPool;
    public Vector3 SpawnPoint { get; private set; }
    public Transform[] PatrolPoint { get; private set; }

    public void SetSpawnPoint(Vector3 position)
    {
        SpawnPoint = position;
        transform.position = SpawnPoint;
    }

    public void SetPatrolPoints(Transform[] patrolPoints)
    {
        PatrolPoint = patrolPoints;
    }
    #endregion
    private void Awake()
    {
        _stateMachine = new EnemyStateMachine();
        RegisterStates();
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    public void OnSpawned()
    {
        DebugManager.Instance.Log("EnemyController OnSpawned");

        Player = PlayerController.Instance;

        Stat.Init();
        Agent.SetAgent(Stat.MoveSpeed.Value);
        _stateMachine.Change(EEnemyState.Spawn);

        EnemyUI.SetActive(true);
    }

    public void OnDespawned()
    {
        DebugManager.Instance.Log("EnemyController OnDespawned");
        transform.position = SpawnPoint;
        SpawnPoint = Vector3.zero;
        PatrolPoint = null;

        Animator.SetBool("IsMoving", false);
        Animator.SetTrigger("Reset");
        OnReturnedToPool?.Invoke(this);
        EnemyUI.SetActive(false);
    }

    private void RegisterStates()
    {
        _stateMachine.Register(EEnemyState.Spawn, new SpawnState(this, _stateMachine));
        _stateMachine.Register(EEnemyState.Idle, new IdleState(this, _stateMachine));
        _stateMachine.Register(EEnemyState.Patrol, new PatrolState(this, _stateMachine));
        _stateMachine.Register(EEnemyState.Trace, new TraceState(this, _stateMachine)); //타겟을 지정하도록 수정
        _stateMachine.Register(EEnemyState.Attack, new AttackState(this, _stateMachine));
        _stateMachine.Register(EEnemyState.Comeback, new ComebackState(this, _stateMachine));
        _stateMachine.Register(EEnemyState.Death, new DeathState(this));
        _stateMachine.Change(EEnemyState.Spawn);
    }

    public void ApplyDamage(AttackData data)
    {
        if (Stat.Health == null || Stat.Health.IsEmpty())
            return;

        Stat.Health.Consume(data.Damage);

        if (Stat.Health.IsEmpty())
        {
            Agent.AgentStop();
            _stateMachine.Change(EEnemyState.Death);
            Animator.SetTrigger("Dead");
        }
        else
        {
            _stateMachine.Change(new HitState(this, _stateMachine,data.HitDirection, data.Knockback.Power));
            Animator.SetTrigger("Hit");
        }

        DebugManager.Instance.Log($"피격 발생 : {data.Attacker.name}가 {gameObject.name}에게 {data.Damage} 데미지. 남은체력 : {Stat.Health.Current}/{Stat.Health.Max}");
    }
}
