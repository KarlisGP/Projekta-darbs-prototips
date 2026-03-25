using UnityEngine;
using System.Collections; // Required for Coroutines
using UnityEngine.Events;

public class PlayerControllerBored : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpDelay = 0.2f; // How long to wait for the animation (e.g., 0.2 seconds)
    private bool facingRight = true;
    private bool isJumpStarting = false; // Prevents double-jumping during the delay

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Boredom Settings")]
    public float timeToWait = 5f; 
    private float idleTimer = 0f;

    [Header("Events")]
    public UnityEvent OnLandEvent; 

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool wasGrounded; 
    private Animator anim;
    private float moveX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (OnLandEvent == null) OnLandEvent = new UnityEvent();
    }

    void Update()
    {
        moveX = Input.GetAxis("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundMask);

        // Landing detection
        if (isGrounded && !wasGrounded)
        {
            OnLanding();
        }
        wasGrounded = isGrounded;

        // 1. Jump Input with Delay
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumpStarting)
        {
            StartCoroutine(JumpRoutine());
        }

        // 2. Flip Sprite Logic
        if (moveX > 0 && !facingRight) Flip();
        else if (moveX < 0 && facingRight) Flip();

        // 3. Boredom Logic
        HandleBoredom();

        // 4. Update Animation Parameters
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("Speed", Mathf.Abs(moveX));
    }

    // This Coroutine handles the "Wind-up" before the jump
    IEnumerator JumpRoutine()
    {
        isJumpStarting = true;
        
        // Play the "JumpStart" or "Crouch" animation trigger
        anim.SetTrigger("JumpStart");

        // Wait for the specified time
        yield return new WaitForSeconds(jumpDelay);

        // Apply the physical jump force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        
        isJumpStarting = false;
    }

    void HandleBoredom()
    {
        if (Mathf.Abs(moveX) < 0.01f && isGrounded && !isJumpStarting)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= timeToWait) anim.SetBool("isBored", true);
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
        idleTimer = 0;
    }

    void FixedUpdate()
    {
        // We only move horizontally if we aren't in the middle of a jump wind-up 
        // (Optional: remove !isJumpStarting if you want to allow sliding while winding up)
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}