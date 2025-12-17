using ArtificeToolkit.Attributes;
using System;
using UnityEngine;


public class EnemyController : MonoBehaviour, IDamageable, IPoolable
{
    [Header("References")]
    [ReadOnly] public PlayerController Player;


    [ReadOnly] public EnemyStat Stat;
    [ReadOnly] public EnemyMove Move;
    [ReadOnly] public EnemyAttack Attack;
    [ReadOnly] public AgentController Agent;
    [ReadOnly] public Animator Animator;

    public GameObject UICanvas;
    // 상태 관리
    [ReadOnly,SerializeField] private EnemyBaseState _currentState;
    [ReadOnly, SerializeField] private Transform[] _patrolPoints;
    [ReadOnly, SerializeField] private Vector3 _spawnPosition;
    public Transform[] PatrolPoints => _patrolPoints;
    public Vector3 SpawnPosition => _spawnPosition;

    public bool SetComplete = false;

    [ReadOnly] public Vector3 LastTracePosition;
    private void Awake()
    {
        Stat=GetComponent<EnemyStat>();
        Move = GetComponent<EnemyMove>();
        Attack = GetComponentInChildren<EnemyAttack>();
        Agent = GetComponent<AgentController>();
        Animator = GetComponentInChildren<Animator>();
    }

    #region Pool
    public EPoolType PoolType => EPoolType.Enemy;
    public event Action<EnemyController> OnReturnedToPool;

    public void Get(EPoolType type)
    {
        Player = PlayerController.Instance;
        Stat.Init();
        Stat.Health.OnCurrentChanged += JudgeDeathInHealthChange;
        Agent.SetAgent(Stat.MoveSpeed.Value);
        TransitionToState(new SpawnState());

        UICanvas.SetActive(true);
    }

    public void Release()
    {
        Stat.Health.OnCurrentChanged -= JudgeDeathInHealthChange;

        transform.position = _spawnPosition;

        _spawnPosition = Vector3.zero;
        _patrolPoints = null;
        SetComplete = false;
        Animator.SetTrigger("Reset");
        OnReturnedToPool?.Invoke(this);
        UICanvas.SetActive(false);
    }

    public void SetPatrolPoints(Transform[] patrolPoints)
    {
        _patrolPoints = patrolPoints;
    }

    public void SetSpawnPoint(Vector3 position)
    {
        _spawnPosition = position;
        transform.position = _spawnPosition;
    }
    #endregion

    

    private void Update()
    {
        _currentState?.Update();
        Stat.Health?.Regenerate(Time.deltaTime);
    }

    public void TransitionToState(EnemyBaseState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter(this);
    }

    private void JudgeDeathInHealthChange(float value)
    {
        if (value <= 0)
        {
            TransitionToState(new DeathState());
        }
    }

    public void ApplyDamage(AttackData data)
    {
        if (Stat.Health == null || Stat.Health.IsEmpty())
            return;

        Stat.Health.Consume(data.Damage);

        if (Stat.Health.IsEmpty())
        {
            Agent.AgentStop();
            TransitionToState(new DeathState());
            Animator.SetTrigger("Dead");
        }
        else
        {
            TransitionToState(new HitState(data.HitDirection, data.Knockback.Power));
            Animator.SetTrigger("Hit");
        }

        DebugManager.Instance.Log($"피격 발생 : {data.Attacker.name}가 {gameObject.name}에게 {data.Damage} 데미지. 남은체력 : {Stat.Health.Current}/{Stat.Health.Max}");
    }
    
}
