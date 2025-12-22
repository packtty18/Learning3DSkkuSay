using DG.Tweening;
using UnityEngine;

public class FPSState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _rotationSpeed;

    private PlayerRotate _playerRotate;
    private float _accumulateY;
    private const int Y_CLAMP = 90;
    private float _tweenTime = 0.35f;
    public FPSState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _rotationSpeed = speed;
    }

    public void Enter()
    {
        FireRebound.OnRecoil += ApplyRecoil;

        _playerRotate = _player.GetComponent<PlayerRotate>();
        _accumulateY = _camera.localEulerAngles.x;


        _camera.DOMove(_pivot.position, _tweenTime).SetEase(Ease.InOutSine);
        _camera.DORotate(new Vector3(_accumulateY, _player.eulerAngles.y, 0f), _tweenTime).SetEase(Ease.InOutSine);
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        float rotateSpeed = _rotationSpeed * Time.deltaTime;

        // 좌우는 플레이어
        _playerRotate.Rotate(mouseX * rotateSpeed);

        // 상하는 카메라
        _accumulateY -= mouseY * rotateSpeed;
        _accumulateY = Mathf.Clamp(_accumulateY, -Y_CLAMP, Y_CLAMP);
    }

    public void SetCameraPosition()
    {
        _camera.position = _pivot.position;
        _camera.rotation = Quaternion.Euler(_accumulateY, _player.eulerAngles.y, 0f);
    }

    public void Exit()
    {
        FireRebound.OnRecoil -= ApplyRecoil;
    }

    private float _xBound = 0f;
    private void ApplyRecoil(RecoilData data)
    {
        // 상하 반동은 영구적 적용
        _accumulateY -= data.YStrength;

        //좌우 반동은 되돌아오는 반동
        float randomSide = UnityEngine.Random.Range(-data.XStrength, data.XStrength);
        _xBound = randomSide;
    }

    public Vector3 GetFireDirection(Transform firePos)
    {
        return _camera.forward;
    }

    
}
