using JetBrains.Annotations;
using UnityEngine;

//마우스에 따른 카메라의 회전
public class CameraRotate : MonoBehaviour
{
    private const int YOFFSET = 90;

    /*
* 기능
* 1. 마우스 입력
* 2. 입력에 따른 회전 방향 만들기
* 3. 회전 방향으로 카메라 회전
*/

    [SerializeField] private float _sensitivity = 200f;

    //변화를 -360~360으로 누적할 변수
    private float _accumulateX = 0;
    private float _accumulateY = 0;

    private void Update()
    {
        if(!Input.GetMouseButton(1))
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity * Time.deltaTime;
        _accumulateX += mouseX * _sensitivity * Time.deltaTime;
        _accumulateY += mouseY * _sensitivity * Time.deltaTime;
        _accumulateY = Mathf.Clamp(_accumulateY, -YOFFSET, YOFFSET);

        DebugManager.Instance.Log($"mouseX = {_accumulateX}, mouseY = {_accumulateY}");

        transform.eulerAngles = new Vector3(-_accumulateY, _accumulateX, 0);

    }
}
