
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 70f;
    [SerializeField] private float deceleration = 90f;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 13f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float coyoteTime = 0.10f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.12f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;

    private Rigidbody2D rb;
    private BoxCollider2D bodyCollider;

    private float moveInput;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float dashCounter;
    private float originalGravityScale;

    private int facingDirection = 1;

    private bool isGrounded;
    private bool wasGrounded;
    private bool isDashing;
    private bool canDash = true;

    private Vector2 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();

        originalGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            facingDirection = moveInput > 0 ? 1 : -1;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)
            && !isGrounded
            && canDash
            && !isDashing)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashCounter -= Time.deltaTime;

            if (dashCounter <= 0f)
            {
                StopDash();
            }
        }
    }

    private void FixedUpdate()
    {
        wasGrounded = isGrounded;

        // Проверяем землю перед расчётом движения.
        CheckGrounded();

        // Переносим игрока на расстояние, которое прошла платформа.
        if (currentPlatform != null && !isDashing)
        {
            rb.position += currentPlatform.PlatformDelta;
            CheckGrounded();
        }

        // Обновляем таймеры прыжка и восстанавливаем dash.
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            canDash = true;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }

        // Если игрок оторвался от платформы, больше не переносим его.
        if (!isGrounded && wasGrounded)
        {
            currentPlatform = null;
        }

        // Во время dash движение контролируется самим dash.
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            return;
        }

        // Прыжок обрабатываем до обычного движения.
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            isGrounded = false;
            currentPlatform = null;
        }

        // Горизонтальное движение.
        float targetMaxSpeed = isGrounded
            && Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : maxSpeed;

        float desiredSpeed = moveInput * targetMaxSpeed;

        float movementRate = Mathf.Abs(moveInput) > 0.01f
            ? acceleration
            : deceleration;

        float calculatedSpeed = Mathf.MoveTowards(
            rb.linearVelocity.x,
            desiredSpeed,
            movementRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(
            calculatedSpeed,
            rb.linearVelocity.y
        );
    }

    private void CheckGrounded()
    {
        Bounds bounds = bodyCollider.bounds;

        Vector2 checkSize = new Vector2(
            bounds.size.x * 0.85f,
            groundCheckDistance
        );

        Vector2 checkPosition = new Vector2(
            bounds.center.x,
            bounds.min.y - groundCheckDistance * 0.5f
        );

        isGrounded = Physics2D.OverlapBox(
            checkPosition,
            checkSize,
            0f,
            groundLayer
        ) != null;
    }

    private void StartDash()
    {
        dashDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (dashDirection.sqrMagnitude < 0.01f)
        {
            dashDirection = new Vector2(facingDirection, 0f);
        }

        dashDirection.Normalize();

        isDashing = true;
        canDash = false;
        dashCounter = dashDuration;

        currentPlatform = null;

        rb.gravityScale = 0f;
        rb.linearVelocity = dashDirection * dashSpeed;
    }

    private void StopDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravityScale;
        rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        MovingPlatform platform =
            collision.collider.GetComponent<MovingPlatform>();

        if (platform == null)
        {
            platform = collision.collider
                .GetComponentInParent<MovingPlatform>();
        }

        if (platform == null)
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                currentPlatform = platform;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        MovingPlatform platform =
            collision.collider.GetComponent<MovingPlatform>();

        if (platform == null)
        {
            platform = collision.collider
                .GetComponentInParent<MovingPlatform>();
        }

        if (platform != null && currentPlatform == platform)
        {
            currentPlatform = null;
        }
    }

    private void OnDisable()
    {
        if (rb != null)
        {
            rb.gravityScale = originalGravityScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        if (box == null)
            return;

        Bounds bounds = box.bounds;

        Vector2 checkSize = new Vector2(
            bounds.size.x * 0.85f,
            groundCheckDistance
        );

        Vector2 checkPosition = new Vector2(
            bounds.center.x,
            bounds.min.y - groundCheckDistance * 0.5f
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(checkPosition, checkSize);
    }
}
