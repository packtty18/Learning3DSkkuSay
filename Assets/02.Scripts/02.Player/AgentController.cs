using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentController : MonoBehaviour
{
    //NavMeshAgent를 관리하는 컨트롤러.
    //agent에 대한 명령은 모두 클래스가 관리하고 플레이어, 적 이동에서 Nav Mesh를 사용할때 참조

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void SetAgent( float moveSpeed, bool InitActive = true)
    {
        _agent.speed = moveSpeed;

        SetEnable(InitActive);
    }

    public void SetEnable(bool active)
    {
        _agent.enabled = active;
    }

    public void SetAgentDestination(Vector3 targetPos)
    {
        _agent.isStopped = false;
        
        _agent.SetDestination(targetPos);
    }

    public void AgentStop(bool stopImmediate = false)
    {
        if(stopImmediate)
        {
            _agent.velocity = Vector3.zero;
        }
        _agent.isStopped = true;
        _agent.ResetPath();
    }

    public void AgentJumpEnd()
    {
        _agent.CompleteOffMeshLink();
        _agent.Warp(transform.position);
        _agent.isStopped = false;
    }

    public void AgentWarp(Vector3 position)
    {
        _agent.Warp(position);
    }

    public OffMeshLinkData GetOffMeshLinkData()
    {
        return _agent.currentOffMeshLinkData;
    }

    public bool IsOffMeshLink()
    {
        return _agent.isOnOffMeshLink;
    }
}
