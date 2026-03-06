using UnityEngine;



namespace Zombies.Spitter.Acids
{
    public class SpitterAcidBall : MonoBehaviour
    {
        [SerializeField] private GameObject acidExplosionPrefab;


        public void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Ground"))
            {
                Instantiate(acidExplosionPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}

