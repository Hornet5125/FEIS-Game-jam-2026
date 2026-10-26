
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class LevelTransition : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string nextSceneName;

    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";

    private bool isLoading;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLoading || !other.CompareTag(playerTag))
            return;

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError("Next Scene Name is empty!", this);
            return;
        }

        isLoading = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
