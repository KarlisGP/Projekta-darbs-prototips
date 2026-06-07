using UnityEngine;

public class BouncyTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportExit;
    public float clearRadius = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (teleportExit != null)
            {
                // 1. Clear the area at the exit so they don't get stuck in a wall
                ClearExitArea();
                
                // 2. Teleport the player
                other.transform.position = teleportExit.position;

                // 3. HARD PHYSICS RESET
                if (rb != null)
                {
                    // Stop all movement
                    rb.linearVelocity = Vector2.zero;
                    
                    // Stop all rotation/spinning
                    rb.angularVelocity = 0f;

                    // This "Sleeps" the physics engine for this object.
                    // It clears any pending forces (like jumps or explosions) 
                    // that were about to happen.
                    rb.Sleep(); 

                    Debug.Log("Player Teleported: All forces and momentum killed.");
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
            if (col.CompareTag("Platform"))
            {
                Destroy(col.gameObject);
            }
        }
    }
}