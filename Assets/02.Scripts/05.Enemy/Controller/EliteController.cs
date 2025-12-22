using UnityEngine;

public class EliteController : EnemyController
{
    public override void OnDespawned()
    {
        base.OnDespawned();
        Animator.SetBool("IsRage", false);
    }
    protected override void RegisterStates()
    {
        StateMachine.Register(EEnemyState.Spawn, new Elite_Spawn(this, StateMachine));
        StateMachine.Register(EEnemyState.Idle, new Elite_Idle(this, StateMachine));
        StateMachine.Register(EEnemyState.Patrol, new Elite_Patrol(this, StateMachine));
        StateMachine.Register(EEnemyState.Trace, new Elite_Trace(this, StateMachine));
        StateMachine.Register(EEnemyState.Attack, new Elite_Attack(this, StateMachine));
        StateMachine.Register(EEnemyState.Comeback, new Elite_Comeback(this, StateMachine));
        StateMachine.Register(EEnemyState.RageOn, new Elite_RageOn(this, StateMachine));
        StateMachine.Register(EEnemyState.RageMove, new Elite_RageMove(this, StateMachine));
        StateMachine.Register(EEnemyState.RageAttack, new Elite_RageAttack(this, StateMachine));
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
            if (IsRage)
            {
                return;
            }
            else
            {
                if (Stat.RageRate.Value > 0 && Stat.Health.GetRatio() <= Stat.RageRate.Value)
                {
                    ChangeRageMode();
                }

                StateMachine.Change(
                    new HitState(this, StateMachine, data.HitDirection, data.Knockback.Power)
                );
                
                Animator.SetTrigger("Hit");
            }
        }

        DebugManager.Instance.Log(
            $"Hit : {data.Attacker.name} -> {gameObject.name} " +
            $"Damage {data.Damage} HP {Stat.Health.Current}/{Stat.Health.Max}"
        );
    }

    private void ChangeRageMode()
    {
        IsRage = true;
        Animator.SetBool("IsRage", true);
    }
}
