
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class FogDeath : MonoBehaviour
{
    private bool isRestarting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isRestarting)
            return;

        if (other.CompareTag("Player"))
        {
            isRestarting = true;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
