using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float deathY = -10f;

    private bool isRestarting;

    private void Update()
    {
        if (!isRestarting && transform.position.y < deathY)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isRestarting)
            return;

        isRestarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
