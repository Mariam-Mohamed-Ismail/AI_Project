using Assets.Scripts.Zombies.Spitter;
using BBUnity.Actions;
using BBUnity.Conditions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;
using UnityEngine.AI;


namespace BBCore.Actions
{
    [Action("Spitter/Retreat")]
    public class RetreatAction : GOAction
    {
        [InParam("Spitter Zombie")]
        private SpitterZombie spitterZombie;

        private NavMeshAgent _navMeshAgent;
        private Vector3 _retreatPosition;
        public override void OnStart()
        {
            if (spitterZombie == null)
                return;

            if (spitterZombie.Animator != null)
                spitterZombie.Animator.SetInteger("state", (int)SpitterState.Running);

            _navMeshAgent = spitterZombie.NavMeshAgent;
            _retreatPosition = spitterZombie.Retreat();
            _navMeshAgent.isStopped = false;

        }

        public override TaskStatus OnUpdate()
        {
            if (spitterZombie == null || spitterZombie.Target == null)
                return TaskStatus.FAILED;

            if (_navMeshAgent == null)
                return TaskStatus.FAILED;

            if(!spitterZombie.IsSpittingFinished)
                return TaskStatus.FAILED;

            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                return TaskStatus.COMPLETED;

            else if (_navMeshAgent.destination != _retreatPosition)
                _navMeshAgent.SetDestination(_retreatPosition);

            return TaskStatus.RUNNING;
        }

        public override void OnAbort()
        {
            if (_navMeshAgent != null)
                _navMeshAgent.isStopped = true;
            if (spitterZombie != null && spitterZombie.Animator != null)
                spitterZombie.Animator.SetInteger("state", (int)SpitterState.Idle);
            base.OnAbort();
        }
        public override void OnEnd()
        {
            if (_navMeshAgent != null)
                _navMeshAgent.isStopped = true;
            if (spitterZombie != null && spitterZombie.Animator != null)
                spitterZombie.Animator.SetInteger("state", (int)SpitterState.Idle);
            base.OnEnd();
        }
    }
}