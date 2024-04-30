using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public void InitNavMeshAgent()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public bool OnReachTarget(Vector3 target, float distance)
    {
        if (Vector3.Distance(target, transform.position) <= distance)
            return true;

        return false;
    }

    public void SetNavMeshTarget(Vector3 target, float speed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.destination = target;
    }

    public void StopNavigation()
    {
        navMeshAgent.speed = 0;
        navMeshAgent.isStopped = true;
    }
    public void ResumeNavigation()
    {
        navMeshAgent.isStopped = false;
    }
}