using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    [Header("Settings")]
    public float slipperyEffect = 0.98f; 

    private Rigidbody2D playerRb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = null;
        }
    }

    private void FixedUpdate()
    {
        // Apply the slide logic here so it stays synced with the physics engine
        if (playerRb != null && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.01f)
        {
            float slideX = playerRb.linearVelocity.x * slipperyEffect;
            playerRb.linearVelocity = new Vector2(slideX, playerRb.linearVelocity.y);
        }
    }
}