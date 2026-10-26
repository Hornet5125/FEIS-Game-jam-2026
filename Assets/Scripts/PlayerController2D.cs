
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 70f;
    [SerializeField] private float deceleration = 1000f;
    [SerializeField] private float sprintSpeed = 13f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float coyoteTime = 0.10f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField, Range(0.1f, 1f)]
    private float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.14f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private float landAnimationDuration = 0.2f;

    private static readonly int IdleState = Animator.StringToHash("Idle");
    private static readonly int RunState = Animator.StringToHash("Run");
    private static readonly int JumpState = Animator.StringToHash("Jump");
    private static readonly int FallState = Animator.StringToHash("Fall");
    private static readonly int DashState = Animator.StringToHash("Dash");
    private static readonly int LandState = Animator.StringToHash("Land");
    private int currentAnimationState = -1;

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
    private bool wasAirborne;
    private float landAnimationCounter;

    private Vector2 dashDirection;
    private MovingPlatform currentPlatform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (characterSprite == null)
            characterSprite = GetComponentInChildren<SpriteRenderer>();

        originalGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        // Получаем горизонтальный ввод.
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0f)
        {
            facingDirection = moveInput > 0f ? 1 : -1;
        }

        if (characterSprite != null)
            characterSprite.flipX = facingDirection < 0;

        // Буфер прыжка: позволяет нажать прыжок немного раньше приземления.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Если отпустить прыжок раньше вершины, прыжок будет ниже.
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }

        // Рывок в воздухе на Left Shift.
        if (Input.GetKeyDown(KeyCode.LeftShift)
            && canDash
            && !isDashing)
        {
            StartDash();
        }

        // Отсчитываем длительность рывка.
        if (isDashing)
        {
            dashCounter -= Time.deltaTime;

            if (dashCounter <= 0f)
            {
                StopDash();
            }
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        int nextState;

        if (isDashing)
            nextState = DashState;
        else if (isGrounded && landAnimationCounter > 0f)
            nextState = LandState;
        else if (!isGrounded)
            nextState = rb.linearVelocity.y > 0.1f ? JumpState : FallState;
        else if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            nextState = RunState;
        else
            nextState = IdleState;

        if (nextState != currentAnimationState)
        {
            currentAnimationState = nextState;
            animator.Play(nextState, 0, 0f);
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        if (isGrounded && wasAirborne && !isDashing)
            landAnimationCounter = landAnimationDuration;

        wasAirborne = !isGrounded;

        if (landAnimationCounter > 0f)
            landAnimationCounter = Mathf.Max(0f, landAnimationCounter - Time.fixedDeltaTime);

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            canDash = true;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }

        // Во время рывка не изменяем скорость движения.
        if (isDashing)
            return;

        float targetSpeed = Input.GetKey(KeyCode.LeftControl)
            ? sprintSpeed
            : maxSpeed;

        float desiredX = moveInput * targetSpeed;

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            // Движение при нажатии клавиш.
            float x = Mathf.MoveTowards(
                rb.linearVelocity.x,
                desiredX,
                acceleration * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector2(
                x,
                rb.linearVelocity.y
            );
        }
        else if (currentPlatform == null)
        {
            // Торможение, когда персонаж не на движущейся платформе.
            float x = Mathf.MoveTowards(
                rb.linearVelocity.x,
                0f,
                deceleration * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector2(
                x,
                rb.linearVelocity.y
            );
        }

        // Прыжок с учётом буфера и времени схода с края.
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
            Mathf.Max(0.02f, bounds.size.x * 0.85f),
            groundCheckDistance
        );

        Vector2 checkPosition = new Vector2(
            bounds.center.x,
            bounds.min.y - groundCheckDistance * 0.45f
        );

        Collider2D hit = Physics2D.OverlapBox(
            checkPosition,
            checkSize,
            0f,
            groundLayer
        );

        isGrounded = hit != null;
    }

    private void StartDash()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");

        dashDirection = new Vector2(moveInput, verticalInput);

        // Если направление не задано, рывок идёт вперёд.
        if (dashDirection.sqrMagnitude < 0.01f)
        {
            dashDirection = new Vector2(facingDirection, 0f);
        }

        dashDirection.Normalize();

        isDashing = true;
        canDash = false;
        dashCounter = dashDuration;

        rb.gravityScale = 0f;
        rb.linearVelocity = dashDirection * dashSpeed;
    }

    private void StopDash()
    {
        isDashing = false;

        rb.gravityScale = originalGravityScale;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            0f
        );
    }

    private void OnDisable()
    {
        // Не оставляем игрока без гравитации,
        // если компонент отключили во время рывка.
        if (rb != null)
        {
            rb.gravityScale = originalGravityScale;
        }

        isDashing = false;
        currentAnimationState = -1;
        wasAirborne = false;
        landAnimationCounter = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        if (box == null)
        {
            return;
        }

        Bounds bounds = box.bounds;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            new Vector2(
                bounds.center.x,
                bounds.min.y - groundCheckDistance * 0.45f
            ),
            new Vector2(
                Mathf.Max(0.02f, bounds.size.x * 0.85f),
                groundCheckDistance
            )
        );
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        MovingPlatform platform =
            collision.gameObject.GetComponent<MovingPlatform>();

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
            collision.gameObject.GetComponent<MovingPlatform>();

        if (platform != null && currentPlatform == platform)
        {
            currentPlatform = null;
        }
    }
}
