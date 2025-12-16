using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class WorldCanvasCameraBinder : MonoBehaviour
{
    [SerializeField] private bool _bindOnEnable = true;

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if (_bindOnEnable)
        {
            Bind();
        }
    }


    public void Bind()
    {
        if (_canvas.renderMode != RenderMode.WorldSpace)
            return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("WorldCanvasCameraBinder : Camera.main not found");
            return;
        }

        _canvas.worldCamera = cam;
    }
}
