using UnityEngine;

public class ManagerRoot : MonoBehaviour
{
    private static ManagerRoot _instance;
    private void Awake()
    {
        if( _instance == null )
        {
            _instance = this; 
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
