using DG.Tweening;
using UnityEngine;
using static UnityEngine.InputSystem.Controls.AxisControl;

//상하회전은 카메라를 기준, 좌우는 피봇을 따라서
public class FPSState : ICameraState
{
    private readonly Transform _camera; //카메라 객체
    private readonly Transform _player; //플레이어 트랜스폼
    private readonly Transform _pivot;  //고정될 위치
    private readonly float _speed;      //회전 속도

    private PlayerRotate _playerRotate;

    private float _accumulateY;
    private float _tweenTime = 0.35f;

    private const int Y_CLAMP = 90;

    private float _xBound = 0f;      
    private float _recoilReturnSpeed = 8f; 


    public FPSState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _speed = speed;
    }

    public void Enter()
    {
        Debug.Log(" Enter FPS Mode");
        FireRebound.OnRecoil += ApplyRecoil;

        _playerRotate = _player.GetComponent<PlayerRotate>();

        _camera.DOMove(_pivot.position, _tweenTime)
            .SetEase(Ease.InOutSine);


        _accumulateY = _camera.localEulerAngles.x;
        // 회전 이동
        _camera.DORotate(new Vector3(_accumulateY, _player.eulerAngles.y, 0f),_tweenTime)
        .SetEase(Ease.InOutSine);
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        float rotateSpeed = _speed * Time.deltaTime;

        //좌우는 플레이어
        _playerRotate.Rotate(mouseX * rotateSpeed);

        // 상하는 카메라
        _camera.position = _pivot.position;
        _accumulateY -= mouseY * rotateSpeed;
        _accumulateY = Mathf.Clamp(_accumulateY, -Y_CLAMP, Y_CLAMP);

        _xBound = Mathf.Lerp(_xBound, 0f, Time.deltaTime * _recoilReturnSpeed);

        // 최종 회전
        _camera.rotation = Quaternion.Euler(_accumulateY, _player.eulerAngles.y + _xBound, 0f);
    }

    public void Exit()
    {
        FireRebound.OnRecoil -= ApplyRecoil;
    }

    private void ApplyRecoil(RecoilData data)
    {
        // 상하 반동은 영구적 적용
        _accumulateY -= data.YStrength;

        //좌우 반동은 되돌아오는 반동
        float randomSide = Random.Range(-data.XStrength, data.XStrength);
        _xBound = randomSide;
    }

    public Vector3 GetFireDirection(Transform firePos)
    {
        return _camera.forward;
    }
}