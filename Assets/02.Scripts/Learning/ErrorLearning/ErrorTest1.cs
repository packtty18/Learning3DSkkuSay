using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ErrorTest1 : MonoBehaviour
{
    public GameObject target;


    private void Start()
    {
        //NullReferenceException : 참조하려는 객체가 Null
        Debug.Log(target.name);


        //MissingComponentException : 참조하려는 컴포넌트가 적용되어 있지 않음
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        if(rigidbody2D == null)
        {
            return;
        }

        //or

        if (TryGetComponent(out Rigidbody2D rigidbody))
        {
            return;
        }

    }
}
