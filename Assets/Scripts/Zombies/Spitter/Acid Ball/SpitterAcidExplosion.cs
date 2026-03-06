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
        StartCoroutine(ExplosionDuration());
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Initial Damage: " + damage);
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            damage += explosionDamageOverTime * Time.deltaTime;
            Debug.Log("Damage Over Time: " + damage);
        }
    }


    private IEnumerator ExplosionDuration()
    {
        yield return new WaitForSeconds(explosionDuration);
        Destroy(gameObject);
    }

}
