using UnityEngine;

public class BouncyTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportExit;
    public float clearRadius = 3f;

    [Header("Bounce Settings (Optional)")]
    public bool giveJumpBoost = true;
    public float bounceForce = 20f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            // 1. Perform Teleport
            if (teleportExit != null)
            {
                ClearExitArea();
                
                // Move the player to the exit
                collision.transform.position = teleportExit.position;

                // 2. STOP MOMENTUM
                if (rb != null)
                {
                    // This sets their movement speed to 0 in all directions
                    rb.linearVelocity = Vector2.zero; 
                    // This stops them from spinning if they were rotating
                    rb.angularVelocity = 0f; 
                }

                Debug.Log("Teleported and Momentum Reset!");

                // 3. Apply Jump Boost (Optional - only if you want them to hop UP at the destination)
                // If you want them to stand perfectly still, move this block BEFORE the Teleport block.
                if (giveJumpBoost && rb != null)
                {
                    rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                    Debug.Log("Post-Teleport Boost Applied!");
                }
            }
        }
    }

    void ClearExitArea()
    {
        if (teleportExit == null) return;

        Collider2D[] collidersAtExit = Physics2D.OverlapCircleAll(teleportExit.position, clearRadius);
        foreach (Collider2D col in collidersAtExit)
        {
            // Only destroy platforms, don't destroy the player!
            if (col.CompareTag("Platform"))
            {
                Destroy(col.gameObject);
            }
        }
    }
}