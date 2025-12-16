using ArtificeToolkit.Attributes;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AI;
public enum ECameraMode
{
    FPS,
    TPS,
    BackView
}
public class CameraStateManager : MonoBehaviour
{
    public event Action OnStateChanged;

    [ReadOnly, SerializeField]
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
        if (GameManager.Instance.State != EGameState.Playing)
            return;

        if (_currentState == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _currentState.UpdateState(mouseX, mouseY);
    }
}





