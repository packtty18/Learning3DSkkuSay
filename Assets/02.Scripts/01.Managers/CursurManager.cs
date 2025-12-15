using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : Singleton<CursorManager>
{
    private bool _isUIActive;

    public override void Init()
    {
        _isUIActive = false; 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursurMode();
        }
    }

    public void SetCursurMode()
    {
        _isUIActive = !_isUIActive;

        if (_isUIActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    
}

