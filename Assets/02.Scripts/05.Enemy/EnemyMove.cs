using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    private EnemyController _controller;
    private AgentController _agent => _controller.Agent;

    [Title("Jump")]
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _jumpDuration = 0.4f;

    private Tween _jumpTween;
    private bool _isJumping;

    [Title("Knockback")]
    private Tween _knockbackTween;
    [ReadOnly,SerializeField] private bool _isKnockback;


    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
            return;

        if (_isJumping)
            return;

        if (_agent.IsOffMeshLink())
        {
            Debug.Log("점프 발생");
            StartJump();
        }
    }

    #region Normal Move

    public void MoveTo(Vector3 targetPos)
    {
        if (_isKnockback)
            return;

        _agent.SetAgentDestination(targetPos);
    }

    

    
    #endregion
    #region Jump
    private void StartJump()
    {
        Debug.Log("Enemy Start OffMeshLink Jump");
        OffMeshLinkData data = _agent.GetOffMeshLinkData(); //ResetPath 전에 가져와야함
        _isJumping = true;
        _agent.AgentStop();
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

        _agent.AgentJumpEnd();
        _isJumping = false;
    }

    #endregion
    #region Knockback

    public void PlayKnockback(Vector3 direction, float power, float duration)
    {
        if (_isKnockback)
            _knockbackTween?.Kill();

        _isKnockback = true;

        _agent.AgentStop();

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
        _agent.AgentWarp(transform.position);
    }

    #endregion

}
