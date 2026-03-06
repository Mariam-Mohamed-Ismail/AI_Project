using UnityEngine;


namespace Zombies.Spitter
{
    public class SpitterZombieHealth : MonoBehaviour
    {

        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private BehaviorExecutor behaviorExecutor;
        [SerializeField] private Animator _animator;
        private float currentHealth;
        public bool IsDead => currentHealth <= 0;

        public void OnTriggerEnter(Collider other)
        {
            TakeDamage(50f);
        }
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            currentHealth = 0;
            if (_animator != null)
                _animator.SetTrigger("die");
            behaviorExecutor.enabled = false;
        }
    }
}
