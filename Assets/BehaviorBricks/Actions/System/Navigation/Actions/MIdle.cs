using UnityEngine;
using Pada1.BBCore.Tasks;
using BBUnity.Actions;
using Pada1.BBCore;
using UnityEngine.AI;

[Action("Zombie/Idle")]
public class Idle : GOAction
{
    private Animator animator;
    private ZombieHealth health;
    private NavMeshAgent agent;

    [InParam("target")] public GameObject target;  
    [InParam("viewDistance")] public float viewDistance = 15f;

    public override void OnStart()
    {
        animator = gameObject.GetComponent<Animator>();
        health = gameObject.GetComponent<ZombieHealth>();
        agent = gameObject.GetComponent<NavMeshAgent>();

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (health == null || target == null || animator == null) return TaskStatus.FAILED;

        if (health.IsDead || health.FirstShotTriggered || health.IsCrawling)
            return TaskStatus.FAILED;

        float distance = Vector3.Distance(gameObject.transform.position, target.transform.position);
        if (distance <= viewDistance)
        {
            return TaskStatus.FAILED; 
        }

        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isCrawling", false);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            animator.Play("Idle");

        return TaskStatus.RUNNING;
    }
}