using UnityEngine;

[ExecuteAlways]
public class BarrelGizmo : MonoBehaviour
{
    [SerializeField] private Barrel _barrel;
    [Header("Gizmo Toggle")]
    [SerializeField] private bool _showRange = true;


    [Header("Colors")]
    [SerializeField] private Color _explosionColor = new Color(1f, 0f, 0f, 0.4f); // 빨강

    private void OnDrawGizmos()
    {
        if (_showRange)
        {
            Gizmos.color = _explosionColor;
            Gizmos.DrawWireSphere(transform.position, _barrel.ExplosionRange.Value);
        }
    }
}
