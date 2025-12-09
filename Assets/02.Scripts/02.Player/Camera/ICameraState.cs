using UnityEngine;

public interface ICameraState
{
    void Enter();
    void UpdateState(float mouseX, float mouseY);
    void Exit();
}
