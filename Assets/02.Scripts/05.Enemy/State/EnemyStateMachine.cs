using System.Collections.Generic;
using static UnityEditorInternal.VersionControl.ListControl;

public class EnemyStateMachine
{
    private IEnemyState _current;
    public EEnemyState LastState;
    private readonly Dictionary<EEnemyState, IEnemyState> _states = new Dictionary<EEnemyState, IEnemyState>();

    public void Register(EEnemyState key, IEnemyState state)
    {
        _states[key] = state;
    }


    public void Change(EEnemyState key)
    {
        if (!_states.ContainsKey(key))
            return;
        
        Change(_states[key]);
    }

    public void Change(IEnemyState state)
    {
        _current?.Exit();
        _current = state;
        _current.Enter();
    }


    public void Update()
    {
        _current?.Update();
    }
}
