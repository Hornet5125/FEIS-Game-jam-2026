
using UnityEngine;

public class ImageKiller : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (DeathScreen.Instance != null)
        {
            DeathScreen.Instance.Die();
        }
        else
        {
            Debug.LogError("DeathScreen не найден в сцене!");
        }
    }
}
