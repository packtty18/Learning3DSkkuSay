using UnityEngine;

[ExecuteAlways]
public class EnemyDistanceDebug : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyState _state;

    [Header("Gizmo Toggle")]
    [SerializeField] private bool _showDetectRange = true;
    [SerializeField] private bool _showAttackRange = true;

    [Header("Colors")]
    [SerializeField] private Color _detectColor = new Color(0f, 0.5f, 1f, 0.4f); // 파랑
    [SerializeField] private Color _attackColor = new Color(1f, 0f, 0f, 0.4f); // 빨강

    private void OnDrawGizmos()
    {
        if (_state == null)
        {
            return;
        }
            
        if (_showDetectRange)
        {
            Gizmos.color = _detectColor;
            Gizmos.DrawWireSphere(transform.position, _state.DetectDistance);
        }

        if (_showAttackRange)
        {
            Gizmos.color = _attackColor;
            Gizmos.DrawWireSphere(transform.position, _state.AttackDistance);
        }
    }
}

