using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    private EnemyController _controller;
    private EnemyStat _enemyStat => _controller.Stat;
    private NavMeshAgent _agent;

    private Tween _knockbackTween;

    private bool _isKnockback;

    //점프
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _jumpDuration = 0.4f;

    private Tween _jumpTween;
    private bool _isJumping;


    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = _enemyStat.MoveSpeed.Value;
    }

    private void Update()
    {
        if (_isJumping)
            return;

        if (_agent.isOnOffMeshLink)
        {
            StartJump();
        }
    }

    #region Normal Move

    public void MoveTo(Vector3 targetPos, float speed)
    {
        if (_isKnockback)
            return;

        _agent.isStopped = false;
        _agent.speed = speed;
        _agent.SetDestination(targetPos);
    }

    public void AgentStopImmediate()
    {
        _agent.velocity = Vector3.zero;
        _agent.isStopped = true;
        _agent.ResetPath();
    }

    #endregion
    #region Jump
    private void StartJump()
    {
        Debug.Log("Enemy Start OffMeshLink Jump");

        _isJumping = true;
        _agent.isStopped = true;
        _agent.updatePosition = false;

        OffMeshLinkData data = _agent.currentOffMeshLinkData;

        Vector3 start = transform.position;
        Vector3 end = data.endPos;

        _jumpTween = DOTween.To(
            () => 0f,
            t =>
            {
                Vector3 pos = Vector3.Lerp(start, end, t);

                float height = 4f * _jumpHeight * t * (1f - t);
                pos.y += height;

                transform.position = pos;
            },
            1f,
            _jumpDuration
        )
        .SetEase(Ease.Linear)
        .OnComplete(EndJump);
    }
    private void EndJump()
    {
        Debug.Log("Enemy End OffMeshLink Jump");

        _agent.CompleteOffMeshLink();

        _agent.updatePosition = true;
        _agent.Warp(transform.position);

        _agent.isStopped = false;
        _isJumping = false;
    }

    #endregion
    #region Knockback

    public void PlayKnockback(Vector3 direction, float power, float duration)
    {
        if (_isKnockback)
            _knockbackTween?.Kill();

        _isKnockback = true;

        _agent.isStopped = true;
        _agent.ResetPath();

        Vector3 dir = direction;
        dir.y = 0f;
        dir.Normalize();

        Vector3 targetPos = transform.position + dir * power;

        _knockbackTween = transform.DOMove(targetPos, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(EndKnockback);
    }

    private void EndKnockback()
    {
        _isKnockback = false;

        _agent.Warp(transform.position);
    }

    #endregion

}
