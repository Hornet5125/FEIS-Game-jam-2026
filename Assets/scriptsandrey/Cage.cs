
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CageTrap : MonoBehaviour
{
    [Header("Cage Settings")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float cageWidth = 8f;
    [SerializeField] private float cageHeight = 8f;
    [SerializeField] private float wallThickness = 0.5f;
    [SerializeField] private bool spawnOnlyOnce = true;

    private bool hasSpawned;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (spawnOnlyOnce && hasSpawned)
            return;

        GameObject cage = new GameObject("Cage");
        cage.transform.position = other.transform.position;

        CreateWall(cage.transform, "Top",
            new Vector2(0, cageHeight / 2),
            new Vector2(cageWidth, wallThickness));

        CreateWall(cage.transform, "Bottom",
            new Vector2(0, -cageHeight / 2),
            new Vector2(cageWidth, wallThickness));

        CreateWall(cage.transform, "Left",
            new Vector2(-cageWidth / 2, 0),
            new Vector2(wallThickness, cageHeight));

        CreateWall(cage.transform, "Right",
            new Vector2(cageWidth / 2, 0),
            new Vector2(wallThickness, cageHeight));

        hasSpawned = true;
    }

    private void CreateWall(
        Transform parent,
        string wallName,
        Vector2 position,
        Vector2 size)
    {
        GameObject wall = wallPrefab != null
            ? Instantiate(wallPrefab, parent)
            : new GameObject(wallName);

        wall.name = wallName;
        wall.transform.localPosition = position;
        wall.transform.localRotation = Quaternion.identity;
        wall.transform.localScale = Vector3.one;

        SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = size;
        }

        BoxCollider2D col = wall.GetComponent<BoxCollider2D>();

        if (col == null)
            col = wall.AddComponent<BoxCollider2D>();

        col.isTrigger = false;
        col.offset = Vector2.zero;
        col.size = size;
    }
}
