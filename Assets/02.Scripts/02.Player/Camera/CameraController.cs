using UnityEngine;

public enum CameraMode
{ 
    FPS,
    TPS,
    BackView
}

public class CameraController : Singleton<CameraController>
{
    [Header("References")]
    [SerializeField] private CameraStateManager _stateMachine;
    [SerializeField] private Transform _player;

    [SerializeField] private CameraMode _defaultMode = CameraMode.FPS;
    [SerializeField] private CameraMode _currentMode;

    public CameraMode CurrentMode => _currentMode;
    public ICameraState CurrentState => _stateMachine.CurrentState;

    [SerializeField] private Transform _fpsPivot;
    [SerializeField] private Transform _tpsPivot;
    [SerializeField] private Transform _backPivot;


    [SerializeField] private float _rotationSpeed = 200;

    public override void Init()
    {
    }
    private void Start()
    {
        ChangeMode(_defaultMode);
        _currentMode = _defaultMode;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            _currentMode = (CameraMode)(((int)_currentMode + 1) % 3);
            ChangeMode(_currentMode);
        }
    }

    public void ChangeMode(CameraMode mode)
    {
        switch (mode)
        {
        case CameraMode.FPS:
                {
                    ChangeToFPS();
                    break;
                }
        case CameraMode.TPS:
                {
                    ChangeToTPS();
                    break;
                }
        case CameraMode.BackView:
                {
                    ChangeToBack();
                    break;
                }
        }
    }

    public void ChangeToFPS()
    {
        _stateMachine.ChangeState(new FPSState(transform, _player, _fpsPivot, _rotationSpeed));
    }

    public void ChangeToTPS()
    {
        _stateMachine.ChangeState(new TPSState(transform, _player, _tpsPivot, _rotationSpeed));
    }

    public void ChangeToBack()
    {
        _stateMachine.ChangeState(new BackViewState(transform, _player, _backPivot, _rotationSpeed));
    }

    
}
