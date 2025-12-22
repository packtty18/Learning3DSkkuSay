using ArtificeToolkit.Editor.Artifice_CustomAttributeDrawers.CustomAttributeDrawer_Validators;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public interface IEnemyState
{
    void Enter();
    void Update();
    void Exit();

}

public abstract class EnemyStateBase : IEnemyState
{
    protected readonly EnemyController _controller;
    protected readonly EnemyStateMachine _machine;

    public EnemyStateBase(EnemyController controller, EnemyStateMachine machine)
    {
        _controller = controller;
        _machine = machine;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
}



#region Spawn
public class SpawnState : EnemyStateBase
{
    public SpawnState(EnemyController controller, EnemyStateMachine machine) : base(controller , machine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Enter SpawnState");
    }

    public override void Update()
    {
        _machine.Change(EEnemyState.Idle);
    }

    public override void Exit() { }
}

#endregion
#region Idle
public class IdleState : EnemyStateBase
{
    public IdleState(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Enter IdleState");
        _controller.Animator.SetBool("IsMoving", false);
    }

    public override void Update()
    {
        if (_controller.Detector.IsDetectRange())
            _machine.Change(EEnemyState.Trace);
    }

    public override void Exit() { }
}

#endregion

#region patrol

public class PatrolState : EnemyStateBase
{
    private int _patrolIndex;

    public PatrolState(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Enter PatrolState");
        _patrolIndex = 0;
        _controller.Animator.SetBool("IsMoving", true);
    }

    public override void Update()
    {
        if (_controller.PatrolPoint == null || _controller.PatrolPoint.Length == 0)
        {
            _machine.Change(EEnemyState.Idle);
            return;
        }

        Transform target = _controller.PatrolPoint[_patrolIndex];
        _controller.Move.MoveTo(target.position, _controller.Stat.MoveSpeed.Value);

        float dist = Vector3.Distance(
            _controller.transform.position,
            target.position
        );

        if (dist <= _controller.Stat.ArrivalThreshold)
        {
            _patrolIndex = (_patrolIndex + 1) % _controller.PatrolPoint.Length;
            _machine.Change(EEnemyState.Idle);
        }

        if (_controller.Detector.IsDetectRange())
        {
            _machine.Change(EEnemyState.Trace);
        }
    }

    public override void Exit() { }
}


#endregion

#region Trace

public class TraceState : EnemyStateBase
{
    public TraceState(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Enter TraceState");
        _controller.Animator.SetBool("IsMoving", true);
    }

    public override void Update()
    {
        _controller.Move.MoveTo(
            PlayerController.Instance.transform.position,  //추후 변경 필요
            _controller.Stat.MoveSpeed.Value
        );

        if (_controller.Detector.IsAttackRange())
            _machine.Change(EEnemyState.Attack);
    }

    public override void Exit() { }
}


#endregion

#region Comeback
public class ComebackState : EnemyStateBase
{
    public ComebackState(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
    {

    }


    public override void Enter()
    {
        Debug.Log("Enter ComebackState");
        _controller.Animator.SetBool("IsMoving", true);
    }

    public override void Update()
    {
        Vector3 target = _controller.SpawnPoint;
        _controller.Move.MoveTo(target, _controller.Stat.MoveSpeed.Value);

        float dist = Vector3.Distance(
            _controller.transform.position,
            target
        );

        if (dist <= 0.3f)
        {
            _machine.Change(EEnemyState.Idle);
        }

        if (_controller.Detector.IsDetectRange())
        {
            _machine.Change(EEnemyState.Trace);
        }
    }

    public override void Exit() { }
}


#endregion

#region Attack
public class AttackState : EnemyStateBase
{
    public AttackState(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
    {

    }

    private float _timer;

    public override void Enter()
    {
        _timer = 0f;
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _controller.Stat.AttackSpeed.Value)
        {
            _controller.Animator.SetTrigger("Attack");
            _timer = 0f;
        }

        if (!_controller.Detector.IsAttackRange())
            _machine.Change(EEnemyState.Trace);
    }

    public override void Exit() { }
}

#endregion

#region Hit
public class HitState : EnemyStateBase
{


    private readonly Vector3 _hitDir;
    private readonly float _power;

    public HitState(
        EnemyController controller,
        EnemyStateMachine machine,
        Vector3 hitDir,
        float power) : base(controller, machine)
    {
        _hitDir = hitDir;
        _power = power;
    }

    public override void Enter()
    {
        Debug.Log("Enter HitState");

        _controller.Animator.SetTrigger("Hit");

        _controller.Move.PlayKnockback(
            _hitDir,
            _power,
            _controller.Stat.KnockbackTime
        );

        _machine.Change(EEnemyState.Idle);
    }

    public override void Update() { }

    public override void Exit() { }
}



#endregion

#region Death
public class DeathState : EnemyStateBase
{

    public DeathState(EnemyController controller) : base(controller, null)
    {
    }

    public override void Enter()
    {
        Debug.Log("Enter DeathState");

        _controller.Animator.SetTrigger("Dead");
        _controller.StartCoroutine(
            Util.DestroyAfterTime(2f, _controller.gameObject)
        );

        for(int i =0; i< 5; i++)
        {
            Coin coin = PoolManager.Instance.Get(EPoolType.Coin).GetComponent<Coin>();
            coin.transform.position = _controller.transform.position;
        }
    }

    public override void Update() { }

    public override void Exit() { }
}


#endregion