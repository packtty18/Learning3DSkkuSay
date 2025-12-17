using ArtificeToolkit.Attributes;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private float _sensitivity = 200f;

    [ReadOnly]private float _accumulateX = 0f;

    public void Rotate(float mouseX)
    {
        _accumulateX += mouseX * _sensitivity * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, _accumulateX, 0);
    }

    public void SetDirection(Vector3 dir)
    {
        transform.forward = dir;
    }

}
