using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance => _instance;

    public void Awake()
    {
        if(_instance == null)
        {
            _instance = GetComponent<T>();
            Init();
        }
        else
        {
            Destroy( this );
        }
    }

    //세팅
    public abstract void Init();
    

    public bool IsExist()
    {
        return _instance != null;
    }
}
