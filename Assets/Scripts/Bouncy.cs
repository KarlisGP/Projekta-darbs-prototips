using UnityEngine;

public class JumpBoost2D : MonoBehaviour
{
    [SerializeField] private float bounceForce = 15f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // CHANGE: Set the ENTIRE velocity to zero.
                // This stops all sideways sliding and all falling speed.
                rb.linearVelocity = Vector2.zero;

                // Launch the player perfectly straight up
                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                
                Debug.Log("Jump Boost: Momentum Reset and Launched Up!");
            }
        }
    }
}