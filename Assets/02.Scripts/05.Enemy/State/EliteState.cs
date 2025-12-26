#region Spawn
using UnityEngine;

public class Elite_Spawn : EnemyStateBase
{
    public Elite_Spawn(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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
public class Elite_Idle : EnemyStateBase
{
    public Elite_Idle(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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

public class Elite_Patrol : EnemyStateBase
{
    private int _patrolIndex;

    public Elite_Patrol(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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

public class Elite_Trace : EnemyStateBase
{
    public Elite_Trace(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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
            PlayerController.Instance.transform.position  //추후 변경 필요
        );

        if (_controller.Detector.IsAttackRange())
            _machine.Change(EEnemyState.Attack);
    }

    public override void Exit() { }
}


#endregion
#region Comeback
public class Elite_Comeback : EnemyStateBase
{
    public Elite_Comeback(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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
public class Elite_Attack : EnemyStateBase
{
    public Elite_Attack(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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
#region RageOn
public class Elite_RageOn : EnemyStateBase
{
    public Elite_RageOn(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Enter RageOn");
        _controller.Animator.SetBool("IsMoving", false);
        _controller.IsInvincible = true;
        _controller.AnimationRunning = true;
        _controller.Agent.AgentStop();
    }

    public override void Update()
    {
        if(_controller.AnimationRunning)
        {
            return; 
        }

        _machine.Change(EEnemyState.RageMove);
    }

    public override void Exit() {
        _controller.Animator.SetBool("IsMoving", true);
        _controller.IsInvincible = false;
    }
}
#endregion
#region RageMove
public class Elite_RageMove : EnemyStateBase
{
    public Elite_RageMove(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Enter RageMove");
        _controller.Animator.SetBool("IsMoving", true);
        _controller.Agent.ChangeSpeed(_controller.Stat.MoveSpeed.Value * 2);
    }

    public override void Update()
    {
        _controller.Move.MoveTo(
            PlayerController.Instance.transform.position
        );

        if (_controller.Detector.IsAttackRange())
            _machine.Change(EEnemyState.RageAttack);
    }

    public override void Exit() { }
}
#endregion
#region RageAttack
public class Elite_RageAttack : EnemyStateBase
{
    public Elite_RageAttack(EnemyController controller, EnemyStateMachine machine) : base(controller, machine)
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

        if (_timer >= _controller.Stat.AttackSpeed.Value /2)
        {
            _controller.Animator.SetTrigger("Attack");
            _timer = 0f;
        }

        if (!_controller.Detector.IsAttackRange())
            _machine.Change(EEnemyState.RageMove);
    }

    public override void Exit() { }
}

#endregion