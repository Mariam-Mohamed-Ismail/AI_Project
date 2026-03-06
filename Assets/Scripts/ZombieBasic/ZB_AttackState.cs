using BBUnity.Actions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;

[Action("Zombie/AttackState")]
public class ZB_AttackState : GOAction
{
    private ZombieBrain brain;
    private float lastAttackTime;

    [InParam("attackCooldown")] public float attackCooldown = 1.2f;

    public override void OnStart()
    {
        brain = gameObject.GetComponent<ZombieBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if (brain == null || brain.player == null) return TaskStatus.FAILED;
        if (brain.IsDead) return TaskStatus.FAILED;

        float dist = Vector3.Distance(gameObject.transform.position, brain.player.position);
        if (dist > brain.attackRange) return TaskStatus.FAILED;

        if (brain.agent != null)
        {
            brain.agent.isStopped = true;
            brain.agent.stoppingDistance = brain.attackRange; 
        }

        Vector3 dir = (brain.player.position - gameObject.transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            gameObject.transform.rotation =
                Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);

        if (brain.anim != null)
        {
            brain.anim.SetBool("IsChasing", false);
            brain.anim.SetFloat("Speed", 0f);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                brain.anim.ResetTrigger("Attack"); 
                brain.anim.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }

        return TaskStatus.RUNNING;
    }

    public override void OnAbort()
    {
        if (brain != null && brain.agent != null)
        {
            brain.agent.isStopped = false;
            brain.agent.stoppingDistance = 0f; 
        }

        if (brain != null && brain.anim != null)
            brain.anim.ResetTrigger("Attack");
    }
}