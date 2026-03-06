using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    public int maxHP = 5;
    public int currentHP;

    public UnityEvent onDeath;

    public bool IsDead => currentHP <= 0;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        // Only play Hit if still alive
        if (currentHP > 0)
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("Hit");
        }

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        onDeath?.Invoke();
        // áÇ ÊÚãá Destroy åäÇ
    }

    public void DestroyAfter(float seconds)
    {
        Destroy(gameObject, seconds);
    }
}