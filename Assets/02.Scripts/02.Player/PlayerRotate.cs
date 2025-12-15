using ArtificeToolkit.Attributes;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private float _sensitivity = 200f;

    [ReadOnly]private float _accumulateX = 0f;
    [ReadOnly, SerializeField] private bool _canRotate = true; // 모드에서 제어

    public void SetRotateEnable(bool enable)
    {
        _canRotate = enable;
    }

    public void Init()
    {
        
    }

    public void Rotate(float mouseX)
    {
        if (!_canRotate) return;

        _accumulateX += mouseX * _sensitivity * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, _accumulateX, 0);
    }

    public void SetDirection(Vector3 dir)
    {
        transform.forward = dir; // 바로 바라보게 설정
    }

}
