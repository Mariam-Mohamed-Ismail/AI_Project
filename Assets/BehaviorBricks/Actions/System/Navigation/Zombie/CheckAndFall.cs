using UnityEngine;
using Pada1.BBCore.Tasks;
using BBUnity.Actions;
using Pada1.BBCore;
using UnityEngine.AI;

[Action("Zombie/CheckAndFall")]
public class CheckAndFall : GOAction
{
    private Animator animator;
    private NavMeshAgent agent;
    private ZombieHealth health;
    private float timer;

    [InParam("fallDuration")] 
    public float fallDuration = 2.0f; 

    private bool fallStarted = false;

    public override void OnStart()
    {
        animator = gameObject.GetComponent<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        health = gameObject.GetComponent<ZombieHealth>();
        timer = 0f;
        fallStarted = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (health == null || animator == null) return TaskStatus.FAILED;
        if (health.IsDead || health.IsCrawling) return TaskStatus.FAILED;

        if (health.FirstShotTriggered && !fallStarted)
        {
            if (agent != null && agent.isOnNavMesh) 
                agent.isStopped = true;

            animator.SetBool("isChasing", false);
            animator.SetBool("isAttacking", false);
            animator.SetTrigger("fallTrigger");
            
            fallStarted = true;
            return TaskStatus.RUNNING;
        }

        if (fallStarted)
        {
            if (agent != null && agent.isOnNavMesh) 
                agent.isStopped = true;

            timer += Time.deltaTime;
            if (timer >= fallDuration)
            {
                health.StartCrawling(); 
                return TaskStatus.COMPLETED;
            }
            return TaskStatus.RUNNING;
        }

        return TaskStatus.FAILED; 
    }
}