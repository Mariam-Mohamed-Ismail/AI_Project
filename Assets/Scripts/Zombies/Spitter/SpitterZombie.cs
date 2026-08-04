using DeliveryDriver.Audio;
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

    public class SpitterZombie : MonoBehaviour, IDamageable
    {
        [Header("Target")]
        [SerializeField] private GameObject target;

        [Header("Main Components")]
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private BehaviorExecutor behaviorExecutor;
        [SerializeField] private Collider zombieCollider;

        // NEW: Reference to the isolated animation handler
        [SerializeField] private SpitterAnimationHandler animationHandler;

        [Header("Spit Attack")]
        [SerializeField] private GameObject acidBallPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float acidBallSpeed = 30f;
        [SerializeField] private float spitCoolDownTime = 7f;
        [SerializeField] private float timeOfFlight = 1.5f;

        [Header("Retreat")]
        [SerializeField] private float retreatMaxDistance = 5f;

        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;

        [Header("Audio")]
        [SerializeField] private AudioSource spittingAudioSource;

        private float _lastSpitTime = 0;
        private float currentHealth;
        private bool _isSpittingFinished = false;

        public NavMeshAgent NavMeshAgent => navMeshAgent;
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

            if (spittingAudioSource != null)
                spittingAudioSource.Play();
        }

        private IEnumerator StartSpitting()
        {
            animationHandler.SetState(SpitterState.Attacking);

            yield return new WaitForSeconds(1f);
            SpitAcid();
            yield return new WaitForSeconds(1.2f);

            _isSpittingFinished = true;

            // Revert to idle animation
            animationHandler.SetState(SpitterState.Idle);
        }

        private void SpitAcid()
        {
            GameObject bullet = Instantiate(acidBallPrefab, firePoint.position, firePoint.rotation);

            Vector3 targetPosition = target.transform.position + Vector3.up * 1.5f;

            Vector3 distance = targetPosition - firePoint.position;

            Vector3 calculatedVelocity = (distance / timeOfFlight) - (0.5f * Physics.gravity * timeOfFlight);

            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = calculatedVelocity;
            }
            else
            {
                bullet.AddComponent<Rigidbody>().linearVelocity = calculatedVelocity;
            }
        }

        private void ResetCoolDown()
        {
            _lastSpitTime = Time.time + spitCoolDownTime;
        }

        public bool IsSpitReady()
        {
            return Time.time >= _lastSpitTime;
        }

        public Vector3 Retreat()
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            Vector3 rightVector = Vector3.Cross(dirToTarget, Vector3.up);

            float randomDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
            Vector3 diagonalEscapeDir = (-dirToTarget + (rightVector * randomDirection)).normalized;

            Vector3 retreatPosition = transform.position + (diagonalEscapeDir * retreatMaxDistance);

            NavMesh.SamplePosition(retreatPosition, out NavMeshHit hit, retreatMaxDistance, NavMesh.AllAreas);
            navMeshAgent.SetDestination(hit.position);

            return hit.position;
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            Debug.Log("Spitter Current Health: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            currentHealth = 0;
            zombieCollider.enabled = false;
            behaviorExecutor.enabled = false;

            animationHandler.TriggerDeath();

            if (spittingAudioSource != null)
                spittingAudioSource.Stop();

            Destroy(gameObject, 5f);
        }
    }
}