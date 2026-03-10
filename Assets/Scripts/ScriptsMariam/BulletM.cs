using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f;

    private Rigidbody rb;
    private bool hasHit = false;

    private void Start()
    {
        Debug.Log("[Bullet] Bullet spawned!");
        Destroy(gameObject, lifetime);

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = transform.forward * speed;
            Debug.Log("[Bullet] Rigidbody found. Velocity set to: " + rb.linearVelocity);
        }
        else
        {
            Debug.LogWarning("[Bullet] NO Rigidbody on bullet! Using transform.Translate fallback.");
        }
    }

    private void Update()
    {
        // Fallback: if no Rigidbody exists, move manually
        if (rb == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Bullet] OnTriggerEnter fired! Hit: " + other.gameObject.name);

        if (hasHit) return;

        ZombieHealth zombie = other.GetComponent<ZombieHealth>();
        if (zombie == null) zombie = other.GetComponentInParent<ZombieHealth>();
        if (zombie == null) zombie = other.GetComponentInChildren<ZombieHealth>();

        if (zombie != null)
        {
            hasHit = true;
            Debug.Log("[Bullet] ZombieHealth found! Calling TakeHit.");
            zombie.TakeHit();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("[Bullet] Hit object has no ZombieHealth component.");
        }
    }

    // Fallback in case the collider is NOT set as trigger
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[Bullet] OnCollisionEnter fired! Hit: " + collision.gameObject.name);

        if (hasHit) return;

        ZombieHealth zombie = collision.gameObject.GetComponent<ZombieHealth>();
        if (zombie == null) zombie = collision.gameObject.GetComponentInParent<ZombieHealth>();
        if (zombie == null) zombie = collision.gameObject.GetComponentInChildren<ZombieHealth>();

        if (zombie != null)
        {
            hasHit = true;
            Debug.Log("[Bullet] ZombieHealth found via collision! Calling TakeHit.");
            zombie.TakeHit();
            Destroy(gameObject);
        }
    }
}