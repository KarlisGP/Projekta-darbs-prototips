using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpDelay = 0.1f;

    private float moveX;
    private bool facingRight = true;
    private bool isJumpStarting = false;

    // NEW: wall movement lock
    private bool movementLocked = false;

    [Header("Extra Jump")]
    public int baseExtraJumps = 0;
    private int extraJumpsAllowed;
    private int extraJumpsRemaining;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip defaultJumpSound;
    public AudioClip airJumpSound;

    [Header("Death Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    private bool isDead = false;
    public AudioClip deathSound;
    public AudioSource bossMusicSource;

    [Header("UI Reference")]
    public GameUIManager uiManager;

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

        extraJumpsAllowed = baseExtraJumps;
        extraJumpsRemaining = extraJumpsAllowed;

        currentHealth = maxHealth;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (uiManager == null)
            uiManager = FindObjectOfType<GameUIManager>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    void Update()
    {
        if (isDead) return;

        // NEW: block input if locked
        moveX = movementLocked ? 0f : Input.GetAxis("Horizontal");

        if (groundCheck == null) return;

        Collider2D groundCollider = Physics2D.OverlapCircle(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        isGrounded = groundCollider != null;
        isOnNoJumpSurface = groundCollider != null && groundCollider.CompareTag(noJumpTag);

        if (isGrounded && !wasGrounded)
            OnLanding();

        wasGrounded = isGrounded;

        if (Input.GetButtonDown("Jump") && !isJumpStarting && !isOnNoJumpSurface)
        {
            if (isGrounded)
                StartCoroutine(JumpRoutine(true, groundCollider));
            else if (extraJumpsRemaining > 0)
            {
                extraJumpsRemaining--;
                StartCoroutine(JumpRoutine(false, null));
            }
        }

        if (moveX > 0 && !facingRight) Flip();
        else if (moveX < 0 && facingRight) Flip();

        HandleBoredom();

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveX));
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("yVelocity", rb.linearVelocity.y);
            anim.SetBool("IsJumping", !isGrounded && Mathf.Abs(rb.linearVelocity.y) > 0.1f);
        }
    }

    IEnumerator JumpRoutine(bool isGroundJump, Collider2D platformCollider)
    {
        isJumpStarting = true;

        if (anim != null)
            anim.SetTrigger("JumpStart");

        if (audioSource != null)
        {
            AudioClip clipToPlay = null;

            if (isGroundJump && platformCollider != null)
            {
                PlatformMaterial platMat = platformCollider.GetComponent<PlatformMaterial>();
                clipToPlay = (platMat != null && platMat.jumpSound != null)
                    ? platMat.jumpSound
                    : defaultJumpSound;
            }
            else if (!isGroundJump)
            {
                clipToPlay = airJumpSound;
            }

            if (clipToPlay != null)
                audioSource.PlayOneShot(clipToPlay);
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

            if (idleTimer >= timeToWait && anim != null)
                anim.SetBool("isBored", true);
        }
        else
        {
            idleTimer = 0f;

            if (anim != null)
                anim.SetBool("isBored", false);
        }
    }

    public void OnLanding()
    {
        OnLandEvent.Invoke();
        idleTimer = 0f;

        extraJumpsAllowed = baseExtraJumps;
        extraJumpsRemaining = extraJumpsAllowed;
    }
    void FixedUpdate()
    {
        if (isDead) return;

        float targetX = moveX * moveSpeed * speedMultiplier;

        float newX = movementLocked
            ? 0f
            : targetX;

        // preserve external physics influences better
        rb.linearVelocity = new Vector2(
            newX,
            rb.linearVelocity.y
        );
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // =========================
    // 🧱 WALL MOVEMENT LOCK
    // =========================

    public void LockMovement()
    {
        movementLocked = true;

        // stop horizontal drift immediately
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void UnlockMovement()
    {
        movementLocked = false;
    }

    // =========================
    // RESET
    // =========================

    public void ResetMovement()
    {
        moveX = 0;
        idleTimer = 0f;
        isJumpStarting = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.Sleep();
        }
    }

    // =========================
    // DAMAGE + DEATH
    // =========================

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        AudioSource[] allAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource s in allAudio) s.Stop();

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);

        if (uiManager != null) uiManager.ShowDeathScreen();

        rb.simulated = false;
        this.enabled = false;
    }

    // =========================
    // BOOST SYSTEM
    // =========================

    public void GiveExtraJump(int amount)
    {
        extraJumpsAllowed = amount;
        extraJumpsRemaining = amount;
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (boostRoutine != null)
            StopCoroutine(boostRoutine);

        boostRoutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
        => TryApplyBoost(collision.gameObject);

    private void OnCollisionEnter2D(Collision2D collision)
        => TryApplyBoost(collision.gameObject);

    void TryApplyBoost(GameObject obj)
    {
        BoostPad pad = obj.GetComponent<BoostPad>();

        if (pad != null)
        {
            ApplySpeedBoost(pad.boostMultiplier, pad.boostDuration);

            Vector2 direction = obj.transform.right.normalized;
            rb.AddForce(direction * pad.pushForce, ForceMode2D.Impulse);

            JumpPadSound padSound = obj.GetComponent<JumpPadSound>();

            if (padSound != null && audioSource != null && padSound.launchSound != null)
            {
                audioSource.PlayOneShot(padSound.launchSound);
            }
        }
    }
}