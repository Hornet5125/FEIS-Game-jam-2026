
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FogSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject fogPrefab;
    [SerializeField] private Camera mainCamera;

    [Header("Spawn Settings")]
    [SerializeField] private float distanceOutsideScreen = 3f;
    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private bool spawnOnlyOnce = true;

    [Header("Movement")]
    [SerializeField] private float fogSpeed = 3f;

    private bool spawned;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (spawnOnlyOnce && spawned)
            return;

        if (fogPrefab == null || mainCamera == null)
            return;

        float screenLeft = mainCamera.transform.position.x
            - mainCamera.orthographicSize * mainCamera.aspect;

        Vector3 spawnPosition = new Vector3(
            screenLeft - distanceOutsideScreen,
            other.transform.position.y + verticalOffset,
            fogPrefab.transform.position.z
        );

        GameObject fog = Instantiate(
            fogPrefab,
            spawnPosition,
            Quaternion.identity
        );

        FogMovement movement = fog.GetComponent<FogMovement>();

        if (movement == null)
            movement = fog.AddComponent<FogMovement>();

        movement.Initialize(other.transform, fogSpeed);

        spawned = true;
    }
}
