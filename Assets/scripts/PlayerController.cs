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

    [Header("Extra Jump")]
    public int extraJumpsAllowed = 0;
    private int extraJumpsRemaining;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip groundJumpSound;
    public AudioClip airJumpSound;

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

        Collider2D groundCollider = Physics2D.OverlapCircle(
            groundCheck.position,
            groundDistance,
            groundMask
        );
        isGrounded = groundCollider != null;

        isOnNoJumpSurface = groundCollider != null && groundCollider.CompareTag(noJumpTag);

        if (isGrounded && !wasGrounded)
        {
            OnLanding();
        }
        wasGrounded = isGrounded;

        // ✅ JUMP LOGIC (ground + mid-air)
        if (Input.GetButtonDown("Jump") && !isJumpStarting && !isOnNoJumpSurface)
        {
            if (isGrounded)
            {
                StartCoroutine(JumpRoutine(true));
            }
            else if (extraJumpsRemaining > 0)
            {
                extraJumpsRemaining--;
                StartCoroutine(JumpRoutine(false));
            }
        }

        if (moveX > 0 && !facingRight) Flip();
        else if (moveX < 0 && facingRight) Flip();

        HandleBoredom();

        anim.SetFloat("Speed", Mathf.Abs(moveX));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        bool isAirborne = !isGrounded && Mathf.Abs(rb.linearVelocity.y) > 0.1f;
        anim.SetBool("IsJumping", isAirborne);
    }

    IEnumerator JumpRoutine(bool isGroundJump)
    {
        isJumpStarting = true;

        anim.SetTrigger("JumpStart");

        // 🔊 PLAY CORRECT SOUND
        if (audioSource != null)
        {
            if (isGroundJump && groundJumpSound != null)
                audioSource.PlayOneShot(groundJumpSound);
            else if (!isGroundJump && airJumpSound != null)
                audioSource.PlayOneShot(airJumpSound);
        }

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

        // ✅ Reset extra jumps
        extraJumpsRemaining = extraJumpsAllowed;
    }

    void FixedUpdate()
    {
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

    // ✅ Called by platform
    public void GiveExtraJump(int amount)
    {
        extraJumpsAllowed = amount;
        extraJumpsRemaining = amount;
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