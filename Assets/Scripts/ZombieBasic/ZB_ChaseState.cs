using BBUnity.Actions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;

[Action("Zombie/ChaseState")]
public class ZB_ChaseState : GOAction
{
    private ZombieBrain brain;

    public override void OnStart()
    {
        brain = gameObject.GetComponent<ZombieBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if (brain == null || brain.player == null) return TaskStatus.FAILED;
        if (brain.IsDead) return TaskStatus.FAILED;
        if (brain.agent == null) return TaskStatus.FAILED;

        float dist = Vector3.Distance(gameObject.transform.position, brain.player.position);

        if (dist > brain.detectRange) return TaskStatus.FAILED;
        if (dist <= brain.attackRange) return TaskStatus.FAILED;

        brain.agent.isStopped = false;
        brain.agent.speed = 3.5f;
        brain.agent.stoppingDistance = brain.attackRange; // ✅ small important
        brain.agent.SetDestination(brain.player.position);

        if (brain.anim != null)
        {
            brain.anim.SetBool("IsChasing", true);
            brain.anim.SetFloat("Speed", brain.agent.velocity.magnitude);
        }

        return TaskStatus.RUNNING;
    }

    public override void OnAbort()
    {
        if (brain != null && brain.anim != null)
            brain.anim.SetBool("IsChasing", false); // ✅ FIX
    }
}