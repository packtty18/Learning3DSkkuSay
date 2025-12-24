using Sirenix.OdinInspector;
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


#region Hit
public class HitState : EnemyStateBase
{
    private readonly Vector3 _hitDir;
    private readonly float _power;

    public HitState(EnemyController controller, EnemyStateMachine machine, Vector3 hitDir, float power) : base(controller, machine)
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

        if(_controller.IsRage)
        {
            _machine.Change(EEnemyState.RageOn);
        }
        else
        {
            _machine.Change(EEnemyState.Idle);
        }
            
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

        for (int i = 0; i < 5; i++)
        {
            Coin coin = PoolManager.Instance.Get(EPoolType.Coin).GetComponent<Coin>();
            coin.transform.position = _controller.transform.position;
        }
    }

    public override void Update() { }

    public override void Exit() { }
}


#endregion