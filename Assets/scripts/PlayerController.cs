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

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.15f;
    public LayerMask groundMask;

    private bool isGrounded;
    private bool wasGrounded;

    [Header("Jump Restrictions")]
    public string noJumpTag = "NoJump"; // 👈 uses tag instead of layer
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

        // ✅ TAG-BASED no jump detection
        isOnNoJumpSurface = groundCollider != null &&
                           groundCollider.CompareTag(noJumpTag);

        // Landing detection
        if (isGrounded && !wasGrounded)
        {
            OnLanding();
        }
        wasGrounded = isGrounded;

        // Jump (blocked if on NoJump tagged object)
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumpStarting && !isOnNoJumpSurface)
        {
            StartCoroutine(JumpRoutine());
        }

        // Flip character
        if (moveX > 0 && !facingRight) Flip();
        else if (moveX < 0 && facingRight) Flip();

        // Bored logic
        HandleBoredom();

        // Animator updates
        anim.SetFloat("Speed", Mathf.Abs(moveX));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isJumping", !isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        // Optional animation flag
        anim.SetBool("isOnNoJumpSurface", isOnNoJumpSurface);
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
        if (Mathf.Abs(moveX) > 0.01f)
        {
            rb.linearVelocity = new Vector2(
                moveX * moveSpeed,
                rb.linearVelocity.y
            );
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}