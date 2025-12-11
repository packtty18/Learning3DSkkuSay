using UnityEngine;
using System;

public class CameraStateManager : MonoBehaviour
{
    public event Action OnStateChanged;

    private ICameraState _currentState;

    public ICameraState CurrentState => _currentState;

    public void ChangeState(ICameraState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
        OnStateChanged?.Invoke();
    }

    private void Update()
    {
        if (_currentState == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _currentState.UpdateState(mouseX, mouseY);
    }
}
