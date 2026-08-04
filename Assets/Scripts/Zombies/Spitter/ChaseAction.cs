using Assets.Scripts.Zombies.Spitter;
using BBUnity.Actions;
using BBUnity.Conditions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;
using UnityEngine.AI;



namespace BBUnity.Actions
{
    [Action("Zombies/Chase")]
    public class ChaseAction : GOAction
    {
        [InParam("Spitter Zombie")]
        private SpitterZombie spitterZombie;

        private Transform targetTransform;
        private NavMeshAgent navAgent;

        public override void OnStart()
        {
            if (spitterZombie == null)
                return;

            targetTransform = spitterZombie.Target.transform;
            navAgent = spitterZombie.NavMeshAgent;
            navAgent.SetDestination(targetTransform.position);
            navAgent.isStopped = false;

        }
        public override TaskStatus OnUpdate()
        {
            if (spitterZombie == null || targetTransform == null)
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

            base.OnEnd();
        }
    }
}
