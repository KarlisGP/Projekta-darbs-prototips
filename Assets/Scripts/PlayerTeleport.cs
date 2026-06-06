using UnityEngine;

public class BouncyTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportExit;
    public float clearRadius = 3f;

    [Header("Bounce Settings (Optional)")]
    public bool giveJumpBoost = true;
    public float bounceForce = 20f;

    // Use Collision instead of Trigger so it works on solid platforms
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            // 1. Apply Jump Boost first (if enabled)
            if (giveJumpBoost && rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                Debug.Log("Boost Applied!");
            }

            // 2. Perform Teleport
            if (teleportExit != null)
            {
                ClearExitArea();
                collision.transform.position = teleportExit.position;
                
                // Optional: stop velocity after teleport so they don't go flying
                // rb.linearVelocity = Vector2.zero; 
                
                Debug.Log("Teleported!");
            }
        }
    }

    void ClearExitArea()
    {
        Collider2D[] collidersAtExit = Physics2D.OverlapCircleAll(teleportExit.position, clearRadius);
        foreach (Collider2D col in collidersAtExit)
        {
            if (col.CompareTag("Platform"))
            {
                Destroy(col.gameObject);
            }
        }
    }
}