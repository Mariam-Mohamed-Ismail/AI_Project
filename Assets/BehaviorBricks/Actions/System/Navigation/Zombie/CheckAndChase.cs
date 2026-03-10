using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore.Tasks;
using BBUnity.Actions;
using Pada1.BBCore;

[Action("Zombie/CheckAndChase")]
public class CheckAndChase : GOAction
{
    [InParam("target")] public GameObject target;
    [InParam("viewDistance")] public float viewDistance = 15f;
    [InParam("attackRange")] public float attackRange = 2f;
    [InParam("speed")] public float speed = 3.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private ZombieHealth health;

    public override void OnStart()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        health = gameObject.GetComponent<ZombieHealth>();
        
        if (animator != null)
        {
            animator.SetBool("isChasing", true);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isCrawling", false);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (health == null || target == null || agent == null || animator == null)
            return TaskStatus.FAILED; 

        if (health.IsDead || health.FirstShotTriggered || health.IsCrawling)
        {
            StopChasing();
            return TaskStatus.FAILED; 
        }

        float dist = Vector3.Distance(agent.transform.position, target.transform.position);

        if (dist > viewDistance)
        {
            StopChasing();
            return TaskStatus.FAILED;
        }

        if (dist <= attackRange)
        {
            StopChasing();
            return TaskStatus.FAILED; 
        }
            
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.speed = speed;
            agent.SetDestination(target.transform.position);
            
            Vector3 direction = (target.transform.position - agent.transform.position).normalized;
            direction.y = 0; 
            if (direction != Vector3.zero)
            {
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, 
                                           Quaternion.LookRotation(direction), Time.deltaTime * 10f);
            }
        }

        if (!animator.GetBool("isChasing"))
            animator.SetBool("isChasing", true); 

        return TaskStatus.RUNNING;
    }

    private void StopChasing()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;

        if (animator != null)
            animator.SetBool("isChasing", false);
    }

    public override void OnEnd()
    {
        StopChasing();
    }
}