
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlatformMusicTrigger : MonoBehaviour
{
    [Header("Platform")]
    [SerializeField] private GameObject purplePlatform;

    [Header("Music")]
    [SerializeField] private AudioSource oldMusic;
    [SerializeField] private AudioClip newMusic;

    [Header("Settings")]
    [SerializeField] private bool activateOnlyOnce = true;

    private bool activated;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (activateOnlyOnce && activated)
            return;

        activated = true;

        if (purplePlatform != null)
            purplePlatform.SetActive(false);

        if (oldMusic != null)
        {
            oldMusic.Stop();

            if (newMusic != null)
            {
                oldMusic.clip = newMusic;
                oldMusic.loop = true;
                oldMusic.Play();
            }
        }
    }
}
