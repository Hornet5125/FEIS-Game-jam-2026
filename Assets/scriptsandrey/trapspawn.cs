
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrapSpawner : MonoBehaviour
{
    [Header("Trap")]
    [SerializeField] private GameObject cagePrefab;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private bool spawnOnlyOnce = true;

    private bool hasSpawned;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (spawnOnlyOnce && hasSpawned)
            return;

        if (cagePrefab == null)
            return;

        Instantiate(
            cagePrefab,
            other.transform.position + spawnOffset,
            Quaternion.identity
        );

        hasSpawned = true;
    }
}
