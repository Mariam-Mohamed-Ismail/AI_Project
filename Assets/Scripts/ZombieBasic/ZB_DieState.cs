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

        if (!fired)
        {
            if (brain.agent != null)
            {
                if (brain.agent.enabled && brain.agent.isOnNavMesh)
                    brain.agent.isStopped = true;

                brain.agent.enabled = false;
            }

            if (brain.anim != null)
            {
                brain.anim.SetBool("IsChasing", false);
                brain.anim.SetFloat("Speed", 0f);
                brain.anim.ResetTrigger("Attack");
                brain.anim.ResetTrigger("Hit");
                brain.anim.ResetTrigger("Die");  
                brain.anim.SetTrigger("Die");
            }

            if (brain.health != null)
                brain.health.DestroyAfter(4f); 

            fired = true;
        }

        return TaskStatus.RUNNING;
    }
}