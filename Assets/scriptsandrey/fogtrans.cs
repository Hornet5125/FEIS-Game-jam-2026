
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class FogLevelTransition : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string nextSceneName = "lvl2";
    [SerializeField] private float contactTime = 5f;

    private float timer;
    private bool playerInside;
    private bool levelLoading;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        if (!playerInside || levelLoading)
            return;

        timer += Time.deltaTime;

        if (timer >= contactTime)
        {
            levelLoading = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            timer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            timer = 0f;
        }
    }
}
