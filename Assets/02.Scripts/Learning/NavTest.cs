using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform Player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(Player.position);
    }
}
