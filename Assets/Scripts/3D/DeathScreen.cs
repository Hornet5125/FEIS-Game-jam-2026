
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen Instance { get; private set; }

    [SerializeField] private GameObject deathOverlay;
    [SerializeField] private float deathScreenDuration = 1f;

    private bool isDead;

    private void Awake()
    {
        Instance = this;

        if (deathOverlay != null)
            deathOverlay.SetActive(false);
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        StartCoroutine(ShowDeathScreen());
    }

    private IEnumerator ShowDeathScreen()
    {
        if (deathOverlay != null)
            deathOverlay.SetActive(true);

        yield return new WaitForSecondsRealtime(deathScreenDuration);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
