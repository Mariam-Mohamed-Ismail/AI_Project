using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Ignore collision between the bullet and the player's own colliders
            Collider[] playerColliders = GetComponentsInChildren<Collider>();
            Collider[] bulletColliders = bullet.GetComponentsInChildren<Collider>();

            foreach (Collider playerCol in playerColliders)
            {
                foreach (Collider bulletCol in bulletColliders)
                {
                    Physics.IgnoreCollision(bulletCol, playerCol);
                }
            }

            Debug.Log("[PlayerShooting] Bullet fired! Player colliders ignored.");
        }
    }
}