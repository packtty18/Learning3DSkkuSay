using UnityEngine;

public class ZombieController : EnemyController
{
    protected override void RegisterStates()
    {
        StateMachine.Register(EEnemyState.Spawn, new Zombie_Spawn(this, StateMachine));
        StateMachine.Register(EEnemyState.Idle, new Zombie_Idle(this, StateMachine));
        StateMachine.Register(EEnemyState.Patrol, new Zombie_Patrol(this, StateMachine));
        StateMachine.Register(EEnemyState.Trace, new ZombieTrace(this, StateMachine));
        StateMachine.Register(EEnemyState.Attack, new Zombie_Attack(this, StateMachine));
        StateMachine.Register(EEnemyState.Comeback, new Zombie_Comeback(this, StateMachine));
        StateMachine.Register(EEnemyState.Death, new DeathState(this));

        StateMachine.Change(EEnemyState.Spawn);
    }

    public override void ApplyDamage(AttackData data)
    {
        if (IsInvincible)
        {
            return;
        }
        if (Stat.Health == null || Stat.Health.IsEmpty())
            return;

        Stat.Health.Consume(data.Damage);

        if (Stat.Health.IsEmpty())
        {
            Agent.AgentStop();
            StateMachine.Change(EEnemyState.Death);
            Animator.SetTrigger("Dead");
        }
        else
        {
            StateMachine.Change(
                new HitState(this, StateMachine, data.HitDirection, data.Knockback.Power)
            );
            Animator.SetTrigger("Hit");
        }

        DebugManager.Instance.Log(
            $"Hit : {data.Attacker.name} -> {gameObject.name} " +
            $"Damage {data.Damage} HP {Stat.Health.Current}/{Stat.Health.Max}"
        );
    }
}
