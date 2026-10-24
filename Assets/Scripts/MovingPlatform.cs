
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 pointA = new Vector3(-3f, 0f, 0f);
    [SerializeField] private Vector3 pointB = new Vector3(3f, 0f, 0f);
    [SerializeField] private float speed = 2f;

    private Rigidbody2D rb;
    private Vector2 worldPointA;
    private Vector2 worldPointB;
    private Vector2 targetPosition;

    private Vector2 lastPosition;
    private Vector2 platformDelta;

    public Vector2 PlatformDelta => platformDelta;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Vector2 startPosition = rb.position;

        worldPointA = startPosition + (Vector2)pointA;
        worldPointB = startPosition + (Vector2)pointB;

        targetPosition = worldPointB;
        lastPosition = rb.position;
    }

    private void FixedUpdate()
    {
        lastPosition = rb.position;

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        platformDelta = nextPosition - lastPosition;

        if (Vector2.Distance(nextPosition, targetPosition) < 0.01f)
        {
            targetPosition = targetPosition == worldPointA
                ? worldPointB
                : worldPointA;
        }
    }
}
