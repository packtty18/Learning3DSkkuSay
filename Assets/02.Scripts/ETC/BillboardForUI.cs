using UnityEngine;

//빌보드 기능
public class BillboardForUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _camera;

    [Header("Options")]
    [SerializeField] private bool _lockX = false;
    [SerializeField] private bool _lockY = false;
    [SerializeField] private bool _lockZ = false;

    private void Awake()
    {
        if (_camera != null)
        {
            return;
        }

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            _camera = mainCam.transform;
        }
    }

    private void LateUpdate()
    {
        if (_camera == null)
        {
            return;
        }

        Vector3 direction = transform.position - _camera.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        Vector3 euler = lookRotation.eulerAngles;

        if (_lockX)
        {
            euler.x = transform.eulerAngles.x;
        }
        if (_lockY)
        {
            euler.y = transform.eulerAngles.y;
        }
        if (_lockZ)
        {
            euler.z = transform.eulerAngles.z;
        }

        transform.rotation = Quaternion.Euler(euler);
    }
}
