using Sirenix.OdinInspector;
using System;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour, IDamageable
{
    [Title("Core References")]
    public PlayerController Player;

    [Title("Enemy Components")]
    [Required] public EnemyStat Stat;
    [Required] public EnemyMove Move;
    [Required] public EnemyAttack Attack;
    [Required] public EnemyDetector Detector;
    [Required] public Animator Animator;
    [Required] public AgentController Agent;

    [Title("UI")]
    [Required] public GameObject EnemyUI;

    protected EnemyStateMachine StateMachine;

    public bool IsRage = false;
    public bool IsInvincible = false;
    public bool AnimationRunning = false;

    public Vector3 SpawnPoint { get; private set; }
    public Transform[] PatrolPoint { get; private set; }

    public event Action<EnemyController> OnReturnedToPool;

    protected virtual void Awake()
    {
        StateMachine = new EnemyStateMachine();
        RegisterStates();
    }

    protected virtual void Update()
    {
        StateMachine.Update();
    }

    public virtual void OnSpawned()
    {
        DebugManager.Instance.Log($"{GetType().Name} OnSpawned");

        Player = PlayerController.Instance;

        Stat.Init();
        Agent.SetAgent(Stat.MoveSpeed.Value);
        StateMachine.Change(EEnemyState.Spawn);

        EnemyUI.SetActive(true);
    }

    public virtual void OnDespawned()
    {
        DebugManager.Instance.Log($"{GetType().Name} OnDespawned");

        transform.position = SpawnPoint;
        SpawnPoint = Vector3.zero;
        PatrolPoint = null;

        Animator.SetBool("IsMoving", false);
        Animator.SetTrigger("Reset");
        IsRage = false;
        IsInvincible = false;
        AnimationRunning = false;

        EnemyUI.SetActive(false);
        OnReturnedToPool?.Invoke(this);
    }

    public void SetSpawnPoint(Vector3 position)
    {
        SpawnPoint = position;
        transform.position = position;
    }

    public void SetPatrolPoints(Transform[] patrolPoints)
    {
        PatrolPoint = patrolPoints;
    }

    protected abstract void RegisterStates();

    public abstract void ApplyDamage(AttackData data);
    
}
