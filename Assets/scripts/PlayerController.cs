using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpDelay = 0.2f;

    private float moveX;
    private bool facingRight = true;
    private bool isJumpStarting = false;

    [Header("Speed Boost")]
    public float speedMultiplier = 1f;
    private Coroutine boostRoutine;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.15f;
    public LayerMask groundMask;

    private bool isGrounded;
    private bool wasGrounded;

    [Header("Jump Restrictions")]
    public string noJumpTag = "NoJump";
    private bool isOnNoJumpSurface;

    [Header("Boredom Settings")]
    public float timeToWait = 5f;
    private float idleTimer = 0f;

    [Header("Events")]
    public UnityEvent OnLandEvent;

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    void Update()
    {
        moveX = Input.GetAxis("Horizontal");

        // Ground check
        Collider2D groundCollider = Physics2D.OverlapCircle(
            groundCheck.position,
            groundDistance,
            groundMask
        );
        isGrounded = groundCollider != null;

        // No-jump detection
        isOnNoJumpSurface = groundCollider != null && groundCollider.CompareTag(noJumpTag);

        // Landing detection
        if (isGrounded && !wasGrounded)
        {
            OnLanding();
        }
        wasGrounded = isGrounded;

        // Jump Input
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumpStarting && !isOnNoJumpSurface)
        {
            StartCoroutine(JumpRoutine());
        }

        // Flip character
        if (moveX > 0 && !facingRight) Flip();
        else if (moveX < 0 && facingRight) Flip();

        HandleBoredom();

        // ==========================================
        // ⚡ ANIMATOR UPDATES (VELOCITY BASED)
        // ==========================================
        
        anim.SetFloat("Speed", Mathf.Abs(moveX));
        anim.SetBool("isGrounded", isGrounded);
        
        // This is the "Y change" logic you asked for:
        // We pass the raw Y velocity. 
        // In the Animator, you can now check if this is > 0.1 (Rising) or < -0.1 (Falling)
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        // Optional: A bool that is true whenever the player is NOT on the ground and moving vertically
        bool isAirborne = !isGrounded && Mathf.Abs(rb.linearVelocity.y) > 0.1f;
        anim.SetBool("IsJumping", isAirborne); 
    }

    IEnumerator JumpRoutine()
    {
        isJumpStarting = true;
        anim.SetTrigger("JumpStart");
        yield return new WaitForSeconds(jumpDelay);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumpStarting = false;
    }

    void HandleBoredom()
    {
        if (Mathf.Abs(moveX) < 0.01f && isGrounded && !isJumpStarting)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= timeToWait)
                anim.SetBool("isBored", true);
        }
        else
        {
            idleTimer = 0f;
            anim.SetBool("isBored", false);
        }
    }

    public void OnLanding()
    {
        OnLandEvent.Invoke();
        idleTimer = 0f;
    }

    void FixedUpdate()
    {
        // Smooth horizontal movement that doesn't break Y physics
        float targetVelocityX = moveX * moveSpeed * speedMultiplier;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (boostRoutine != null) StopCoroutine(boostRoutine);
        boostRoutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision) => TryApplyBoost(collision.gameObject);
    private void OnCollisionEnter2D(Collision2D collision) => TryApplyBoost(collision.gameObject);

    void TryApplyBoost(GameObject obj)
    {
        BoostPad pad = obj.GetComponent<BoostPad>();
        if (pad != null)
        {
            ApplySpeedBoost(pad.boostMultiplier, pad.boostDuration);
            Vector2 direction = obj.transform.right.normalized;
            rb.AddForce(direction * pad.pushForce, ForceMode2D.Impulse);
        }
    }
}