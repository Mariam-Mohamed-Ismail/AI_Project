using BBUnity.Actions;
using BBUnity.Conditions;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using UnityEngine;
using UnityEngine.AI;


namespace BBCore.Actions
{
    [Action("Zombies/Retreat")]
    public class RetreatAction : GOAction
    {
        [InParam("target")]
        [SerializeField] private GameObject target;
        [InParam("Retreat Max Distance")]
        [SerializeField] private float retreatMaxDistance = 5f;
        private NavMeshAgent _navMeshAgent;
        private Vector3 _retreatPosition;
        private Animator _animator;
        public override void OnStart()
        {
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
             Retreat();
            _navMeshAgent.isStopped = false;
           _animator = gameObject.GetComponent<Animator>();
            if (_animator != null)
                _animator.SetInteger("state", (int)ZombieState.Running);
        }

        public override TaskStatus OnUpdate()
        {
          if(target == null)
                return TaskStatus.FAILED;

          if(_navMeshAgent == null)
                return TaskStatus.FAILED;

            if(!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                return TaskStatus.COMPLETED;
            else if (_navMeshAgent.destination != _retreatPosition)
                _navMeshAgent.SetDestination(_retreatPosition);

            return TaskStatus.RUNNING;
        }
        private Vector3 Retreat()
        {
           Vector3 retreatDirection = (gameObject.transform.position - target.transform.position).normalized;
           Vector3 retreatPosition = retreatDirection * retreatMaxDistance + gameObject.transform.position;
           NavMesh.SamplePosition(retreatPosition, out NavMeshHit hit, retreatMaxDistance, NavMesh.AllAreas);
          _navMeshAgent.SetDestination(hit.position);
            return hit.position;    
        }

        public override void OnAbort()
        {
            _navMeshAgent.isStopped = true;
            base.OnAbort();
        }

        public override void OnEnd()
        {
            if (_animator != null)
                _animator.SetInteger("state", (int)ZombieState.Idle);
            base.OnEnd();
        }
    }
}