using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

//피봇의 위치만 따라감
public class TPSState : ICameraState
{
    private readonly Transform _camera;
    private readonly Transform _player;
    private readonly Transform _pivot;
    private readonly float _speed;

    private PlayerRotate _playerRotate;

    private float tweenTime = 0.35f;

    public TPSState(Transform cam, Transform player, Transform pivot, float speed)
    {
        _camera = cam;
        _player = player;
        _pivot = pivot;
        _speed = speed;
    }

    public void Enter()
    {
        DebugManager.Instance.Log("Enter TPS Mode");
        FireRebound.OnRecoil += ApplyRecoil;


        _playerRotate = _player.GetComponent<PlayerRotate>();

        
        _camera.DOMove(_pivot.position, tweenTime).SetEase(Ease.InOutSine);
        _camera.DORotate(_pivot.eulerAngles, tweenTime)
            .SetEase(Ease.InOutSine);
    }

    public void UpdateState(float mouseX, float mouseY)
    {
        float rotateSpeed = _speed * Time.deltaTime;

        
        _playerRotate.Rotate(mouseX * rotateSpeed);
        _camera.position = _pivot.position;
    }

    public void Exit()
    {
        FireRebound.OnRecoil -= ApplyRecoil;
    }

    private void ApplyRecoil(RecoilData data)
    {
        
    }

    public Vector3 GetFireDirection(Transform firePos)
    {
        return _pivot.forward;
    }
}
