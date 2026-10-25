
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformDropThrough : MonoBehaviour
{
    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private float checkHeight = 0.2f;

    private Rigidbody2D rb;
    private bool isDropping;

    private void Awake()
    {
        if (playerCollider == null)
            playerCollider = GetComponent<BoxCollider2D>();

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDropping || playerCollider == null)
            return;

        if (Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            TryDropThrough();
        }
    }

    private void TryDropThrough()
    {
        Bounds bounds = playerCollider.bounds;

        Vector2 boxSize = new Vector2(
            bounds.size.x * 0.8f,
            checkHeight
        );

        Vector2 boxCenter = new Vector2(
            bounds.center.x,
            bounds.min.y + checkHeight * 0.5f
        );

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            boxCenter,
            boxSize,
            0f
        );

        foreach (Collider2D hit in hits)
        {
            if (hit == playerCollider || !hit.enabled)
                continue;

            PlatformEffector2D effector =
                hit.GetComponent<PlatformEffector2D>();

            if (effector == null)
                effector = hit.GetComponentInParent<PlatformEffector2D>();

            if (effector == null || !effector.enabled)
                continue;

            StartCoroutine(DropThrough(hit));
            return;
        }
    }

    private IEnumerator DropThrough(Collider2D platform)
    {
        isDropping = true;

        Physics2D.IgnoreCollision(
            playerCollider, platform, true
        );

        // Немного толкаем игрока вниз, чтобы он сразу начал падать.
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Min(rb.linearVelocity.y, -1f)
            );
        }

        float elapsed = 0f;
        float timeout = 5f;

        // Ждём, пока весь игрок окажется ниже платформы.
        while (platform != null &&
               playerCollider != null &&
               playerCollider.bounds.max.y >= platform.bounds.min.y &&
               elapsed < timeout)
        {
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (platform != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(
                playerCollider, platform, false
            );
        }

        isDropping = false;
    }
}
