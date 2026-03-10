using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    [SerializeField] private int maxHealth = 100;

    private int _currentHealth;   
    private Collider _collider;
    void Start()
    {
        _currentHealth = maxHealth;
        _collider = GetComponent<Collider>();
    }

    public void TakeDamage(int amount)
    {
        if(_currentHealth <=0 )
        {
            Die();
            return;
        }
        _currentHealth -= amount;   
    }

    private void Die()
    {
        _collider.enabled = false;
       GameSceneManager.Instance.ReloadLevel();
    }
}
