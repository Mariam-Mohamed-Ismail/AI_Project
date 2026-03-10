using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Zombies.Spitter
{
    public enum SpitterState
    {
        Idle,
        Running,
        Attacking
    }
    public class SpitterZombie :MonoBehaviour,IDamageable
    {
        [Header("Target")]
        [SerializeField] private GameObject target;

        [Header("Main Components")]
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private BehaviorExecutor behaviorExecutor;
        [SerializeField] private Collider colider;

        [Header("Spit Attack")]
        [SerializeField] private GameObject acidBallPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float acidBallSpeed =30f;
        [SerializeField] private float spitCoolDownTime = 7f;

        [Header ("Retreat")]
        [SerializeField] private float retreatMaxDistance = 5f;

        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;

        private float _lastSpitTime = 0;
        private float currentHealth;
        private bool _isSpittingFinished = false;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public Animator Animator => animator;   
        public BehaviorExecutor BehaviorExecutor => behaviorExecutor;
        public GameObject Target => target;
        public bool IsDead => currentHealth <= 0;
        public bool IsSpittingFinished => _isSpittingFinished;
       private void Awake()
        {
            currentHealth = maxHealth;
        }
        public void Spit()
        {
            _isSpittingFinished = false;
            ResetCoolDown();
            StartCoroutine(StartSpitting());
        }

        private IEnumerator StartSpitting()
        {
            yield return new WaitForSeconds(1f);
                SpitAcid();
            yield return new WaitForSeconds(1.2f);
            _isSpittingFinished = true;
         animator.SetInteger("state", (int)SpitterState.Idle);
        }
        private void SpitAcid()
        {
            GameObject bullet = Instantiate(acidBallPrefab, firePoint.position, firePoint.rotation);
            Vector3 dirNormalized = (target.transform.position - firePoint.position).normalized;

            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb.linearVelocity = dirNormalized * acidBallSpeed;
            else
                bullet.AddComponent<Rigidbody>().linearVelocity = dirNormalized * acidBallSpeed;
        }
        private void ResetCoolDown()
        {
            _lastSpitTime += spitCoolDownTime;
        }
        public bool IsSpitReady()
        {
            return Time.time >= _lastSpitTime;
        }
        public Vector3 Retreat()
        {
            Vector3 retreatDirection = (gameObject.transform.position - target.transform.position).normalized;
            Vector3 retreatPosition = retreatDirection * retreatMaxDistance + gameObject.transform.position;
            NavMesh.SamplePosition(retreatPosition, out NavMeshHit hit, retreatMaxDistance, NavMesh.AllAreas);
            navMeshAgent.SetDestination(hit.position);
            return hit.position;
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            Debug.Log("Spitter Current Health: " + currentHealth);
        }
        public void Die()
        {
            currentHealth = 0;
            colider.enabled = false;
            behaviorExecutor.enabled = false;
            if (animator != null)
                animator.SetTrigger("die");
            Destroy(gameObject, 5f);
        }
    }
}
