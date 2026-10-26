using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NextLocationTrigger : MonoBehaviour
{
    [Header("Next Location")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private string locationTitle = "ЛОКАЦИЯ 02";
    [SerializeField] private float textDuration = 1.5f;
    [SerializeField] private string playerTag = "Player";

    [Header("TextMeshPro")]
    [SerializeField] private TMP_Text locationText;
    [SerializeField] private TMP_FontAsset locationFont;

    private bool isTransitioning;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        StartTransition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        StartTransition();
    }

    private void StartTransition()
    {
        if (isTransitioning)
            return;

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError("Не указана следующая сцена!", this);
            return;
        }

        isTransitioning = true;
        StartCoroutine(TransitionToNextLocation());
    }

    private IEnumerator TransitionToNextLocation()
    {
        if (locationText != null)
        {
            if (locationFont != null)
                locationText.font = locationFont;

            locationText.text = locationTitle;
            locationText.gameObject.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(textDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}
