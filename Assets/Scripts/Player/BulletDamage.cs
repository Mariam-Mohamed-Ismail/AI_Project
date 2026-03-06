using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public int damage = 1;

    // Layer that can receive damage
    public string enemyLayerName = "Enemy";

    private int enemyLayer;

    void Start()
    {
        enemyLayer = LayerMask.NameToLayer(enemyLayerName);
    }

    void OnCollisionEnter(Collision collision)
    {
        TryDamage(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    void TryDamage(Collider hit)
    {
        // Only damage objects on Enemy layer
        if (hit.gameObject.layer != enemyLayer)
            return;

        // If target implements IDamageable
        if (hit.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(damage);
        }
        else
        {
            // Sometimes collider is on a child
            var parentDmg = hit.GetComponentInParent<IDamageable>();

            if (parentDmg != null)
                parentDmg.TakeDamage(damage);
        }
    }
}