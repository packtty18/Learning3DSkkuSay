
#region Spawn
using UnityEngine;

public class Zombie_Spawn : EnemyStateBase
{
    public Zombie_Spawn(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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
public class Zombie_Idle : EnemyStateBase
{
    public Zombie_Idle(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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

public class Zombie_Patrol : EnemyStateBase
{
    private int _patrolIndex;

    public Zombie_Patrol(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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
        _controller.Move.MoveTo(target.position);

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

public class ZombieTrace : EnemyStateBase
{
    public ZombieTrace(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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
            PlayerController.Instance.transform.position
        );

        if (_controller.Detector.IsAttackRange())
            _machine.Change(EEnemyState.Attack);
    }

    public override void Exit() { }
}


#endregion
#region Comeback
public class Zombie_Comeback : EnemyStateBase
{
    public Zombie_Comeback(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
    {

    }


    public override void Enter()
    {
        Debug.Log("Enter ComebackState");
        _controller.Animator.SetBool("IsMoving", true);
    }

    public override void Update()
    {
        Vector3 target = _controller.SpawnPoint.position;
        _controller.Move.MoveTo(target);

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
public class Zombie_Attack : EnemyStateBase
{
    public Zombie_Attack(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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

