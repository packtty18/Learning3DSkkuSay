using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemyDetector : MonoBehaviour
{
    private EnemyController _controller;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }


    public bool IsDetectRange()
    {
        float dist = GetDistance(_controller.Player.transform);
        return dist <= _controller.Stat.DetectDistance.Value;
    }

    private float GetDistance(Transform target)
    {
        return Vector3.Distance(_controller.transform.position, target.position);
    }

    public bool IsAttackRange()
    {
        float dist = GetDistance(_controller.Player.transform);
        return dist <= _controller.Stat.AttackDistance.Value;
    }
}
