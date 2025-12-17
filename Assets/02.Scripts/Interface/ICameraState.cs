using UnityEngine;

public interface ICameraState
{
    void Enter();
    void UpdateState(float mouseX, float mouseY);
    void Exit();

    void SetCameraPosition();
    //사격의 기준 방향
    Vector3 GetFireDirection(Transform firePos);
}
