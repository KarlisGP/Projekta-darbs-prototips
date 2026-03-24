using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public bool canJump = true;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    [Header("Boredom Settings")]
    public float timeToWait = 5f; // Seconds of being still before bored
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
        // 1. Ground check
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        // 2. Get Input
        float moveX = Input.GetAxis("Horizontal");

        // 3. Jump Logic (Modified with your canJump check)
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }

        // 4. Boredom Timer Logic
        // We only count if the player is standing still on the ground
        if (moveX == 0 && isGrounded)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= timeToWait)
            {
                // Switches from Idle 1 to Idle 2
                anim.SetBool("isBored", true);
            }
        }
        else
        {
            // Reset timer and go back to Idle 1/Run if they move or jump
            idleTimer = 0f;
            anim.SetBool("isBored", false);
        }

        // 5. General Animation Parameters
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

        Debug.Log("Grounded: " + isGrounded);
    }

    // Detection for "NoJumpPlatform"
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NoJumpPlatform"))
        {
            canJump = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NoJumpPlatform"))
        {
            canJump = true;
        }
    }
}