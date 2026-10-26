
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NextLocationTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private string locationTitle = "ЛОКАЦИЯ 02";
    [SerializeField] private TMP_Text locationText;
    [SerializeField] private float textDuration = 1.5f;
    [SerializeField] private string playerTag = "Player";

    private bool isTransitioning;

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning || !other.CompareTag(playerTag))
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
            locationText.gameObject.SetActive(true);
            locationText.text = locationTitle;
        }

        yield return new WaitForSecondsRealtime(textDuration);

        SceneManager.LoadScene(nextSceneName);
    }
}
