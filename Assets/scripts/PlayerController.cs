using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float groundAcceleration = 15f;
    public float airAcceleration = 8f;

    [Header("Jumping")]
    public float jumpForce = 16f;
    public float fallGravityMultiplier = 2.5f;   // Snappier fall
    public float lowJumpMultiplier = 2f;          // Short-hop when button released early

    [Header("Wall Interaction")]
    public float wallSlideSpeed = 2f;             // Max fall speed while wall sliding
    public float wallJumpForce = 12f;
    public float wallJumpHorizontalForce = 8f;
    public float wallJumpLockTime = 0.15f;        // Briefly locks horizontal input after wall jump

    [Header("Ground & Wall Detection")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public Transform groundCheck;
    public Transform wallCheck;
    public float groundCheckRadius = 0.15f;
    public float wallCheckDistance = 0.4f;

    // ── Internal state ──────────────────────────────────────────────
    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool facingRight = true;

    private float wallJumpLockTimer;   // Counts down after a wall jump

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        GatherInput();
        CheckCollisions();
        HandleWallSlide();
        HandleJump();
        HandleFlip();
    }

    void FixedUpdate()
    {
        HandleMovement();
        ApplyGravityModifiers();
    }

    // ── Input ────────────────────────────────────────────────────────
    void GatherInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (wallJumpLockTimer > 0f)
            wallJumpLockTimer -= Time.deltaTime;
    }

    // ── Collision checks ─────────────────────────────────────────────
    void CheckCollisions()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Cast a short ray in the direction the player faces to detect walls
        Vector2 wallDir = facingRight ? Vector2.right : Vector2.left;
        isTouchingWall = Physics2D.Raycast(wallCheck.position, wallDir, wallCheckDistance, wallLayer);
    }

    // ── Horizontal movement ──────────────────────────────────────────
    void HandleMovement()
    {
        // Suppress horizontal control briefly after a wall jump
        if (wallJumpLockTimer > 0f) return;

        float accel = isGrounded ? groundAcceleration : airAcceleration;
        float targetVelocityX = horizontalInput * moveSpeed;

        rb.linearVelocity = new Vector2(
            Mathf.MoveTowards(rb.linearVelocity.x, targetVelocityX, accel * Time.fixedDeltaTime),
            rb.linearVelocity.y
        );
    }

    // ── Jump & wall jump ─────────────────────────────────────────────
    void HandleJump()
    {
        if (!Input.GetButtonDown("Jump")) return;

        if (isGrounded)
        {
            Jump(Vector2.up, jumpForce);
        }
        else if (isWallSliding)
        {
            // Push away from the wall
            float pushDir = facingRight ? -1f : 1f;
            Vector2 wallJumpDir = new Vector2(pushDir, 1f).normalized;

            rb.linearVelocity = new Vector2(pushDir * wallJumpHorizontalForce, wallJumpForce);
            wallJumpLockTimer = wallJumpLockTime;
            Flip(); // Face away from wall
        }
    }

    void Jump(Vector2 direction, float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset vertical velocity first
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    // ── Wall slide ───────────────────────────────────────────────────
    void HandleWallSlide()
    {
        bool canWallSlide = isTouchingWall && !isGrounded && rb.linearVelocity.y < 0f;

        if (canWallSlide)
        {
            isWallSliding = true;
            // Clamp downward speed to create a slow slide effect
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,
                Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    // ── Gravity modifiers ────────────────────────────────────────────
    void ApplyGravityModifiers()
    {
        if (isWallSliding) return; // Wall slide handles its own gravity clamping

        if (rb.linearVelocity.y < 0f)
        {
            // Fall faster for a snappier feel
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            // Player released jump early → cut the jump short
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    // ── Sprite flipping ──────────────────────────────────────────────
    void HandleFlip()
    {
        if (wallJumpLockTimer > 0f) return;

        if (horizontalInput > 0f && !facingRight) Flip();
        else if (horizontalInput < 0f && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    // ── Editor helpers ───────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Vector2 wallDir = facingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheck.position, (Vector2)wallCheck.position + wallDir * wallCheckDistance);
        }
    }
}
