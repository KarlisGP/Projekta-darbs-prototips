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

    [Header("Extra Jump")]
    public int baseExtraJumps = 0;
    private int extraJumpsAllowed;
    private int extraJumpsRemaining;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip defaultJumpSound;
    public AudioClip airJumpSound;

    [Header("Death")]
    public float maxHealth = 100f;
    public float currentHealth;
    private bool isDead = false;

    public AudioClip deathSound;

    // 🎵 IMPORTANT: assign the AudioSource that plays boss music here
    public AudioSource bossMusicSource;

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

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    void Update()
    {
        if (isDead) return;

        moveX = Input.GetAxis("Horizontal");

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

        if (Mathf.Abs(moveX) > 0.01f)
        {
            rb.linearVelocity = new Vector2(
                moveX * moveSpeed * speedMultiplier,
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

    // =========================
    // 💀 DAMAGE + DEATH SYSTEM
    // =========================

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        Debug.Log($"Player HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("Player died!");

        // 🛑 STOP BOSS MUSIC (SAFE CHECK)
        if (bossMusicSource != null)
        {
            bossMusicSource.Stop();
        }
        else
        {
            Debug.LogWarning("Boss music source NOT assigned!");
        }

        // 🔊 PLAY DEATH SOUND (WILL NOT GET CUT OFF)
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        else
        {
            Debug.LogWarning("Death sound not assigned!");
        }

        rb.linearVelocity = Vector2.zero;

        StartCoroutine(DisablePlayer());
    }

    private IEnumerator DisablePlayer()
    {
        yield return new WaitForSeconds(0.05f);
        this.enabled = false;
    }

    // =========================
    // BOOST SYSTEM (UNCHANGED)
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