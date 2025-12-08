using UnityEngine;

public class DebugManager : Singleton<DebugManager>
{
    [SerializeField]private bool _showDebug = true;
    public override void Init()
    {
        _showDebug = true;
    }

    public void Log(string log)
    {
        if(!_showDebug)
        {
            return;
        }

        Debug.Log(log);
    }

    public void LogError(string log)
    {
        if (!_showDebug)
        {
            return;
        }

        Debug.LogError(log);
    }

    public void LogWarning(string log)
    {
        if (!_showDebug)
        {
            return;
        }

        Debug.LogWarning(log);
    }

}
