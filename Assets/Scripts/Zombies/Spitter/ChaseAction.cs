using BBUnity.Actions;
using BBUnity.Conditions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;
using UnityEngine.AI;

public enum ZombieState
{
    Idle,
    Running,
    Attacking
}

namespace BBUnity.Actions
{
    [Action("Zombies/Chase")]
    public class ChaseAction : GOAction
    {
        [InParam("target")]
        public GameObject target;

        private NavMeshAgent navAgent;
        private Transform targetTransform;
        private Animator animator;

        public override void OnStart()
        {
            if (target == null)
                return;
            targetTransform = target.transform;

            navAgent = gameObject.GetComponent<NavMeshAgent>();

            if (navAgent == null)
                navAgent = gameObject.AddComponent<NavMeshAgent>();
           
            navAgent.SetDestination(targetTransform.position);
            navAgent.isStopped = false;
            animator = gameObject.GetComponent<Animator>();
            if (animator != null)
                animator.SetInteger("state", (int)ZombieState.Running);
        }

       
        public override TaskStatus OnUpdate()
        {
            if (target == null)
                return TaskStatus.FAILED;
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
                return TaskStatus.COMPLETED;
            else if (navAgent.destination != targetTransform.position)
                navAgent.SetDestination(targetTransform.position);
            return TaskStatus.RUNNING;
        }
        public override void OnAbort()
        {
            if (navAgent != null)
                navAgent.isStopped = true;
            base.OnAbort();

        }
        public override void OnEnd()
        {
            if (navAgent != null)
                navAgent.isStopped = true;
            if (animator != null)
                animator.SetInteger("state", (int)ZombieState.Idle);
            base.OnEnd();
        }
    }
}
