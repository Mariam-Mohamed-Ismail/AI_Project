using System.Collections;
using UnityEngine;

public class SpitterAcidExplosion : MonoBehaviour
{
    [SerializeField] private float explosionInitialDamage = 20f;
    [SerializeField] private float explosionDamageOverTime = 3f;
    [SerializeField] private float explosionDuration = 5f;


    private float damage = 0;
    public void OnEnable()
    {
        damage = explosionInitialDamage;
        ExplosionDuration();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            damage = explosionInitialDamage;  
    }
    public void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            damage += explosionDamageOverTime * Time.deltaTime;

            if(other.TryGetComponent<IDamageable>(out IDamageable playerDamageable))
                playerDamageable.TakeDamage((int) damage);  
        }
    }
    private void ExplosionDuration()
    {
        Destroy(gameObject , explosionDuration);
    }

}
