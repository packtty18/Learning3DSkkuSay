using System.Collections;
using UnityEngine;


public abstract class EnemyBaseState
{
    protected EnemyController _controller;
    public virtual void Enter(EnemyController controller)
    {
        _controller = controller;
    }
    public abstract void Update();
    public virtual void Exit() 
    { 

    }
}
#region Spawn
public class SpawnState : EnemyBaseState
{
    public override void Enter(EnemyController controller)
    {
        base.Enter(controller);
        DebugManager.Instance.Log("Enter SpawnState");
    }

    public override void Update()
    {
        if (!_controller.SetComplete)
            return;
        _controller.transform.position = _controller.SpawnPosition;

        _controller.TransitionToState(new IdleState());

    }
}
#endregion
#region Idle
public class IdleState : EnemyBaseState
{
    private float idleTimer = 0f;

    public override void Enter(EnemyController controller)
    {
        base.Enter(controller);
        idleTimer = 0f;
        DebugManager.Instance.Log("Enter IdleState");
    }

    public override void Update()
    {
        float distance = Vector3.Distance(_controller.transform.position, _controller.Player.transform.position);

        if (distance <= _controller.Stat.DetectDistance.Value)
        {
            _controller.TransitionToState(new TraceState());
            _controller.Animator.SetBool("IsMoving", true);
            return;
        }

        idleTimer += Time.deltaTime;
        if (_controller.PatrolPoints.Length > 0 && idleTimer >= _controller.Stat.IdleWaitTime)
        {
            _controller.TransitionToState(new PatrolState());
            _controller.Animator.SetBool("IsMoving", true);
        }
    }
}
#endregion

#region patrol
public class PatrolState : EnemyBaseState
{
    private int patrolIndex = 0;

    public override void Enter(EnemyController _controller)
    {
        base.Enter(_controller);
        DebugManager.Instance.Log("Enter PatrolState");
        patrolIndex = 0;
    }

    public override void Update()
    {
        if (_controller.PatrolPoints.Length == 0) 
            return;

        Transform targetPoint = _controller.PatrolPoints[patrolIndex];
        _controller.Move.MoveTo(targetPoint.position, _controller.Stat.MoveSpeed.Value);

        float distance = Vector3.Distance(_controller.transform.position, targetPoint.position);
        if (distance < _controller.Stat.ArrivalThreshold)
        {
            patrolIndex = (patrolIndex + 1) % _controller.PatrolPoints.Length;
            _controller.TransitionToState(new IdleState());
            _controller.Animator.SetBool("IsMoving", false);
        }

        // 플레이어 발견 시 Trace 상태
        float playerDistance = Vector3.Distance(_controller.transform.position, _controller.Player.transform.position);
        if (playerDistance <= _controller.Stat.DetectDistance.Value)
        {
            _controller.TransitionToState(new TraceState());
            _controller.Animator.SetBool("IsMoving", true);
        }
    }
}

#endregion

#region Trace
public class TraceState : EnemyBaseState
{
    public override void Enter(EnemyController _controller)
    {
        base.Enter(_controller);
        _controller.LastTracePosition = _controller.transform.position;
        DebugManager.Instance.Log("Enter TraceState");
    }

    public override void Update()
    {
        float distance = Vector3.Distance(_controller.transform.position, _controller.Player.transform.position);

        _controller.Move.MoveTo(_controller.Player.transform.position, _controller.Stat.MoveSpeed.Value);

        if (distance <= _controller.Stat.AttackDistance.Value)
        {
            _controller.TransitionToState(new AttackState());
            _controller.Animator.SetBool("IsMoving", false);
            _controller.Animator.SetTrigger("Attack");
        }

        if (distance > _controller.Stat.DetectDistance.Value * 1.2f)
        {
            _controller.TransitionToState(new ComebackState());
            _controller.Animator.SetBool("IsMoving", true);
        }
    }
}

#endregion

#region Comeback
public class ComebackState : EnemyBaseState
{
    public override void Enter(EnemyController _controller)
    {
        base.Enter(_controller);
        DebugManager.Instance.Log("Enter ComebackState");
    }

    public override void Update()
    {
        Vector3 targetPos = _controller.SpawnPosition;
        if (_controller.LastTracePosition != Vector3.zero && _controller.PatrolPoints.Length > 0)
        {
            targetPos = _controller.LastTracePosition;
        }

        _controller.Move.MoveTo(targetPos, _controller.Stat.MoveSpeed.Value);

        if (Vector3.Distance(_controller.transform.position, targetPos) < 0.3f)
        {
            _controller.LastTracePosition = Vector3.zero;
            _controller.TransitionToState(new IdleState());
            _controller.Animator.SetBool("IsMoving", false);
        }

        float playerDistance = Vector3.Distance(_controller.transform.position, _controller.Player.transform.position);
        if (playerDistance <= _controller.Stat.DetectDistance.Value)
        {
            _controller.TransitionToState(new TraceState());
            _controller.Animator.SetBool("IsMoving", true);
        }
    }
}

#endregion

#region Attack
public class AttackState : EnemyBaseState
{
    private float attackTimer = 0f;

    public override void Enter(EnemyController enemy)
    {
        base.Enter(enemy);
        attackTimer = 0f;
        DebugManager.Instance.Log("Enter AttackState");
    }

    public override void Update()
    {
        float distance = Vector3.Distance(_controller.transform.position, _controller.Player.transform.position);
        if (distance > _controller.Stat.AttackDistance.Value)
        {
            _controller.TransitionToState(new TraceState());
            return;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= _controller.Stat.AttackSpeed.Value)
        {
            _controller.Animator.SetTrigger("Attack");
            attackTimer = 0;
        }
    }
}

#endregion

#region Hit
public class HitState : EnemyBaseState
{
    private Vector3 _dir;
    private float _power;

    public HitState(Vector3 dir, float power)
    {
        _dir = dir;
        _power = power;
    }

    public override void Enter(EnemyController controller)
    {
        base.Enter(controller);

        float knockDistance = _power;
        float knockDuration = controller.Stat.KnockbackTime;

        controller.Move.PlayKnockback(
            _dir,
            knockDistance,
            knockDuration
        );

        _controller.TransitionToState(new IdleState());
    }

    public override void Update() { }
}


#endregion

#region Death
public class DeathState : EnemyBaseState
{
    public override void Enter(EnemyController _controller)
    {
        base.Enter(_controller);
        _controller.StartCoroutine(Util.DestroyAfterTime(2f, _controller.gameObject));
    }

    public override void Update() { }
}

#endregion