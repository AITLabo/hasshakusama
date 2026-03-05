using UnityEngine;
using UnityEngine.AI;

public class Hachisha_NPCFriend : MonoBehaviour
{
    public Transform player;
    public Transform hachisha;
    public float followDistance = 3f;
    public float fleeDistance = 10f;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    private NavMeshAgent agent;
    private bool isFleeing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distToHachisha = hachisha != null ? Vector3.Distance(transform.position, hachisha.position) : float.MaxValue;

        if (distToHachisha < fleeDistance)
        {
            FleeFromHachisha();
        }
        else
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        isFleeing = false;
        agent.speed = walkSpeed;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer > followDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    void FleeFromHachisha()
    {
        isFleeing = true;
        agent.speed = runSpeed;
        agent.isStopped = false;

        // 八尺様から反対方向に逃げる
        Vector3 fleeDirection = (transform.position - hachisha.position).normalized;
        Vector3 fleePos = transform.position + fleeDirection * 10f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePos, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
