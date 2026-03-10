using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject player;

    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, 40, player.transform.position.z);
    }
}
