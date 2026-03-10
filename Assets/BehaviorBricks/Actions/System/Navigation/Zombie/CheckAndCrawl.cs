using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore.Tasks;
using BBUnity.Actions;
using Pada1.BBCore;

[Action("Zombie/CheckAndCrawl")]
public class CheckAndCrawl : GOAction
{
    [InParam("target")] public GameObject target;

    private NavMeshAgent agent;
    private Animator animator;
    private ZombieHealth health;

    public float crawlSpeed = 1.5f;

    public override void OnStart()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        health = gameObject.GetComponent<ZombieHealth>();

        if (animator != null)
        {
            animator.SetBool("isChasing", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isCrawling", true);
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = crawlSpeed;
            agent.isStopped = false;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (health == null || agent == null || animator == null)
            return TaskStatus.FAILED;

        if (health.IsDead)
            return TaskStatus.FAILED; 

        if (!health.IsCrawling)
            return TaskStatus.FAILED;

        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.transform.position);
        }

        return TaskStatus.RUNNING; 
    }

    public override void OnEnd()
    {
        if (animator != null)
            animator.SetBool("isCrawling", false);

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
    }
}