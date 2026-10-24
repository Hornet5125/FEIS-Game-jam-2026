
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 70f;
    [SerializeField] private float deceleration = 90f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float coyoteTime = 0.10f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.08f;

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
        // Read horizontal input: A/D or arrow keys.
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            facingDirection = moveInput > 0 ? 1 : -1;
        }

        // Remember a jump press for a short time.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Reduce jump height when the player releases Space early.
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
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
        CheckGrounded();

        // Coyote time: allow jumping just after leaving a ledge.
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            canDash = true;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }

        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            return;
        }

        // Smoothly accelerate and decelerate horizontally.
        float targetSpeed = moveInput * maxSpeed;
        float rate = Mathf.Abs(moveInput) > 0.01f
            ? acceleration
            : deceleration;

        float newSpeed = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            rate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);

        // Jump buffer + coyote time.
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }
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

        // Dash forward if no direction is pressed.
        if (dashDirection.sqrMagnitude < 0.01f)
        {
            dashDirection = new Vector2(facingDirection, 0f);
        }

        dashDirection.Normalize();

        isDashing = true;
        canDash = false;
        dashCounter = dashDuration;

        // Disable gravity while dashing.
        rb.gravityScale = 0f;
        rb.linearVelocity = dashDirection * dashSpeed;
    }

    private void StopDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravityScale;

        // Stop the dash's momentum. Gravity takes over afterward.
        rb.linearVelocity = Vector2.zero;
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
        {
            return;
        }

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
