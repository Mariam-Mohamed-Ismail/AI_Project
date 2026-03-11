using UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable

{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private ProgressBarUI progressBar;

    private int _currentHealth;

    private Collider _collider;
    void Start()
    {
        _currentHealth = maxHealth;
        _collider = GetComponent<Collider>();
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (_currentHealth <= 0)
        {
            Die();
            return;
        }


        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        UpdateHealthUI();

        if (_currentHealth == 0)
        {
            Die();
        }

        _currentHealth -= amount;

    }

    private void UpdateHealthUI()
    {
        float healthPercent = (float)_currentHealth / maxHealth;
        progressBar.SetAmount(healthPercent);
    }
    private void Die()
    {

        _collider.enabled = false;
        GameSceneManager.Instance.ReloadLevel();
    }
}
