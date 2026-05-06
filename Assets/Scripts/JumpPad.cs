using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Settings")]
    public float launchForce = 15f;
    public AudioClip launchSound;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the thing that hit the pad is the Player
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // 1. Reset the Y velocity so the bounce is consistent 
                // (prevents weird jumps if player is already falling fast)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

                // 2. Apply the upward force
                rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);

                // 3. Play the jump pad sound using the player's AudioSource
                if (player.audioSource != null && launchSound != null)
                {
                    player.audioSource.PlayOneShot(launchSound);
                }
                
                // 4. Optional: Tell the player's animator to play the jump animation
                Animator anim = player.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetTrigger("JumpStart");
                }
            }
        }
    }
}