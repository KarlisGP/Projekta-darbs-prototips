using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    [Header("Settings")]
    public float iceSpeedBoost = 1.5f; // Makes the player faster on ice
    public float slipperyEffect = 0.98f; // How much speed is KEPT (0.99 = very slippery)

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();

            // If the player stops pressing buttons, don't let them stop instantly
            if (Input.GetAxis("Horizontal") == 0)
            {
                // Manually keep the player moving slightly
                float slideX = playerRb.linearVelocity.x * slipperyEffect;
                playerRb.linearVelocity = new Vector2(slideX, playerRb.linearVelocity.y);
            }
        }
    }
}