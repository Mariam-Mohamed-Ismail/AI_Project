using BBUnity.Actions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;

[Action("Zombie/DieState")]
public class ZB_DieState : GOAction
{
    private ZombieBrain brain;
    private bool fired;

    public override void OnStart()
    {
        brain = gameObject.GetComponent<ZombieBrain>();
        fired = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (brain == null) return TaskStatus.FAILED;
        if (!brain.IsDead) return TaskStatus.FAILED;

        // Do death logic ONCE
        if (!fired)
        {
            // Stop agent safely
            if (brain.agent != null)
            {
                if (brain.agent.enabled && brain.agent.isOnNavMesh)
                    brain.agent.isStopped = true;

                brain.agent.enabled = false;
            }

            // Play death animation once
            if (brain.anim != null)
            {
                brain.anim.SetBool("IsChasing", false);
                brain.anim.SetFloat("Speed", 0f);
                brain.anim.ResetTrigger("Attack");
                brain.anim.ResetTrigger("Hit");
                brain.anim.ResetTrigger("Die");   // safety
                brain.anim.SetTrigger("Die");
            }

            // Destroy later (increase to test)
            if (brain.health != null)
                brain.health.DestroyAfter(4f); // use 4f for testing, then back to 2f

            fired = true;
        }

        return TaskStatus.RUNNING;
    }
}