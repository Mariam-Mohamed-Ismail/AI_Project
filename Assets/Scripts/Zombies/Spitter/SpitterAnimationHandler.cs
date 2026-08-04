using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Zombies.Spitter
{
    [RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
    public class SpitterAnimationHandler : MonoBehaviour
    {
        private Animator _animator;
        private NavMeshAgent _agent;

        private readonly int _stateHash = Animator.StringToHash("state");
        private readonly int _dieHash = Animator.StringToHash("die");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (_animator.GetInteger(_stateHash) != (int)SpitterState.Attacking)
            {
                if (_agent.velocity.magnitude > 0.1f)
                {
                    SetState(SpitterState.Running);
                }
                else
                {
                    SetState(SpitterState.Idle);
                }
            }
        }

        public void SetState(SpitterState newState)
        {
            if (_animator != null)
            {
                _animator.SetInteger(_stateHash, (int)newState);
            }
        }

        public void TriggerDeath()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(_dieHash);
            }
        }
    }
}