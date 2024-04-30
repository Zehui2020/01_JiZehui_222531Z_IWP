using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Vector3 target;

    public void InitNavMeshAgent()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public bool OnReachTarget(float distance)
    {
        if (Vector3.Distance(target, transform.position) <= distance)
            return true;

        return false;
    }

    public void SetNavMeshTarget(Vector3 target, float speed)
    {
        this.target = target;
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