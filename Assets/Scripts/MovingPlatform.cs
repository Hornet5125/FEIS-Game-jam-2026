using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement relative to starting position")]
    [SerializeField] private Vector2 pointA = new Vector2(-3f, 0f);
    [SerializeField] private Vector2 pointB = new Vector2(3f, 0f);
    [SerializeField, Min(0.01f)] private float speed = 2f;
    [SerializeField, Min(0f)] private float pauseAtPoint = 0f;

    private Rigidbody2D rb;
    private Vector2 worldPointA;
    private Vector2 worldPointB;
    private Vector2 targetPosition;
    private Vector2 platformDelta;
    private float pauseTimer;

    public Vector2 PlatformDelta => platformDelta;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Vector2 start = rb.position;
        worldPointA = start + pointA;
        worldPointB = start + pointB;
        // Move from the placed position toward B first, then alternate A/B.
        targetPosition = worldPointB;
    }

    private void FixedUpdate()
    {
        Vector2 previous = rb.position;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            platformDelta = Vector2.zero;
            return;
        }

        Vector2 next = Vector2.MoveTowards(previous, targetPosition, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);
        platformDelta = next - previous;

        if ((next - targetPosition).sqrMagnitude < 0.0001f)
        {
            targetPosition = (targetPosition - worldPointA).sqrMagnitude < 0.0001f
                ? worldPointB
                : worldPointA;
            pauseTimer = pauseAtPoint;
        }
    }
}
