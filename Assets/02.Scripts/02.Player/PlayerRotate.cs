using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private float _sensitivity = 200f;

    //변화를 -360~360으로 누적할 변수
    private float _accumulateX = 0;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;
        _accumulateX += mouseX * _sensitivity * Time.deltaTime;

        transform.eulerAngles = new Vector3(0, _accumulateX, 0);

    }
}
