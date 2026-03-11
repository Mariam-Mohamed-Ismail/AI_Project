using UnityEngine;

public class TriggerWin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameSceneManager.Instance.LoadMainMenu();
        }
    }
}
