using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    [Header("Boredom Settings")]
    public float timeToWait = 5f; // Set this in the Inspector (e.g., 5 seconds)
    private float idleTimer = 0f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Ground check logic
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        // Inside Update()
        float moveX = Input.GetAxis("Horizontal");

// Check if moveX is ALMOST zero (better than == 0)
        if (Mathf.Abs(moveX) < 0.01f && isGrounded) 
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= timeToWait)
            {
                anim.SetBool("isBored", true);
            }
        }
        else
        {
            idleTimer = 0f;
            anim.SetBool("isBored", false);
        }

        // 3. Jump Logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }

        // 4. Boredom Timer Logic (From your reference)
        // We only count if the player is not moving and touching the ground
        if (moveX == 0 && isGrounded)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= timeToWait)
            {
                // Transition from Idle 1 to Idle 2
                anim.SetBool("isBored", true);
            }
        }
        else
        {
            // If we move, jump, or fall, reset everything
            // Transition from Idle 2 back to Idle 1 or Run
            idleTimer = 0f;
            anim.SetBool("isBored", false);
        }

        // 5. Update Animation Parameters
        anim.SetBool("isJumping", !isGrounded);
        anim.SetFloat("Speed", Mathf.Abs(moveX)); 
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(
            moveX * moveSpeed,
            rb.linearVelocity.y
        );
    }
}