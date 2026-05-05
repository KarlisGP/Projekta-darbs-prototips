using UnityEngine;

public class JumpBoost2D : MonoBehaviour
{
    [SerializeField] private float bounceForce = 15f;

    // In 2D, we MUST use OnCollisionEnter2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Check if the object is tagged Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. Get the 2D Rigidbody
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // 3. Reset the velocity so the bounce height is always the same
                // even if the player is falling very fast.
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

                // 4. Launch the player! 
                // Use Vector2.up for "Straight Up" 
                // Use transform.up if you want to launch based on platform tilt
                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            }
        }
    }
}