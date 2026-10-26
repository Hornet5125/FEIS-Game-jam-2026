
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool spawnOnlyOnce = true;

    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";

    private bool hasSpawned;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (spawnOnlyOnce && hasSpawned)
            return;

        SpawnObjects();
        hasSpawned = true;
    }

    private void SpawnObjects()
    {
        if (objectsToSpawn.Length == 0 ||
            spawnPoints.Length == 0)
            return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject prefab =
                objectsToSpawn[i % objectsToSpawn.Length];

            if (prefab == null || spawnPoints[i] == null)
                continue;

            Instantiate(
                prefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );
        }
    }
}
