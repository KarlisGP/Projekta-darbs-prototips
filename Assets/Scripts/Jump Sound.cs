using UnityEngine;

public class JumpSoundHandler : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb; 
    public AudioSource audioSource;
    public AudioClip jumpSound;

    [Header("Ground Check Settings")]
    [Tooltip("Match these to your PlayerController settings")]
    public Transform groundCheck;
    public float groundDistance = 0.15f;
    public LayerMask groundMask;

    private bool isGrounded;
    private bool wasGrounded;

    void Update()
    {
        // 1. Check ground state independently to stay in sync
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, 
            groundDistance, 
            groundMask
        ) != null;

        // 2. Detect the moment of "Lift Off"
        // If we were on ground last frame, but not this frame, 
        // AND our velocity is going UP (to distinguish jumping from falling)
        if (wasGrounded && !isGrounded && rb.linearVelocity.y > 0.1f)
        {
            PlayJumpSound();
        }

        // Store state for next frame
        wasGrounded = isGrounded;
    }

    void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
        {
            // Stop current sound if it's somehow still playing to prevent overlap
            audioSource.Stop(); 
            audioSource.clip = jumpSound;
            audioSource.Play();
        }
    }
}