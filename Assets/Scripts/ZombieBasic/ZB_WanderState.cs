using BBUnity.Actions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;
using UnityEngine.AI;

[Action("Zombie/WanderState")]
public class ZB_WanderState : GOAction
{
    private ZombieBrain brain;
    private float timer;

    [InParam("wanderRadius")] public float wanderRadius = 6f;
    [InParam("wanderWait")] public float wanderWait = 2f;

    public override void OnStart()
    {
        brain = gameObject.GetComponent<ZombieBrain>();
        timer = wanderWait;
    }

    public override TaskStatus OnUpdate()
    {
        if (brain == null || brain.agent == null) return TaskStatus.FAILED;
        if (brain.IsDead) return TaskStatus.FAILED;

        // if player is near -> let chase/attack run
        if (brain.player != null)
        {
            float dist = Vector3.Distance(gameObject.transform.position, brain.player.position);
            if (dist <= brain.detectRange) return TaskStatus.FAILED;
        }

        timer += Time.deltaTime;

        if (timer >= wanderWait || brain.agent.remainingDistance <= 0.5f)
        {
            Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
            randomDir += gameObject.transform.position;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                brain.agent.isStopped = false;
                brain.agent.stoppingDistance = 0f;
                brain.agent.speed = 1.2f;   
                brain.agent.SetDestination(hit.position);
            }

            timer = 0f;
        }

        if (brain.anim != null)
        {
            brain.anim.SetBool("IsChasing", false); 
            float spd = brain.agent.velocity.magnitude;
            brain.anim.SetFloat("Speed", spd);
        }
        return TaskStatus.RUNNING;
    }

    public override void OnAbort()
    {
        if (brain != null && brain.anim != null)
            brain.anim.SetFloat("Speed", 0f);

    }
}