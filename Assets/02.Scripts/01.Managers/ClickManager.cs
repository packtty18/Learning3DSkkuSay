using System;
using UnityEngine;


//클릭 매니저의 역할은 게임에서 마우스 왼쪽 클릭 혹은 오른족 클릭을 각각 몇번했는지 추적한다.
public class ClickManager : Singleton<ClickManager>
{
    private int _leftCount = 0;
    private int _rightCount = 0;

    public int LeftClickCount => _leftCount;
    public int RightClickCount => _rightCount;


    public event Action<int> OnLeftClick;
    public event Action<int> OnRightClick;
   
    public override void Init()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _leftCount++;
            OnLeftClick?.Invoke(_leftCount);
        }
        if(Input.GetMouseButtonDown(2))
        {
            _rightCount++;
            OnRightClick?.Invoke(_rightCount);
        }
    }
}
