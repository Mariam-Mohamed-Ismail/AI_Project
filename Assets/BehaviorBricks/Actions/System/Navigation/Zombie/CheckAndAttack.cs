using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore.Tasks;
using BBUnity.Actions;
using Pada1.BBCore;

[Action("Zombie/CheckAndAttack")]
public class CheckAndAttack : GOAction
{
    [InParam("target")] public GameObject target;
    [InParam("attackRange")] public float attackRange = 2f;
    [InParam("attackDuration")] public float attackDuration = 1.2f;

    private Animator animator;
    private NavMeshAgent agent;
    private ZombieHealth health;

    private float timer = 0f;
    private bool attackStarted = false;

    public override void OnStart()
    {
        animator = gameObject.GetComponent<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        health = gameObject.GetComponent<ZombieHealth>();

        timer = 0f;
        attackStarted = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (animator == null || agent == null || health == null || target == null)
            return TaskStatus.FAILED;

        if (health.FirstShotTriggered || health.IsDead || health.IsCrawling)
            return TaskStatus.FAILED;

        float distance = Vector3.Distance(agent.transform.position, target.transform.position);

        if (distance > attackRange)
            return TaskStatus.FAILED;

        if (!attackStarted)
        {
            animator.SetBool("isChasing", false);
            animator.SetBool("isCrawling", false);
            animator.SetBool("isAttacking", true);
            if (agent.isOnNavMesh) agent.isStopped = true;
            attackStarted = true;
            timer = 0f;
        }

        Vector3 direction = (target.transform.position - agent.transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation,
                                       Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }

        timer += Time.deltaTime;
        if (timer >= attackDuration)
        {
            return TaskStatus.COMPLETED;
        }

        return TaskStatus.RUNNING;
    }

    public override void OnEnd()
    {
        if (animator != null)
            animator.SetBool("isAttacking", false);
    }
}