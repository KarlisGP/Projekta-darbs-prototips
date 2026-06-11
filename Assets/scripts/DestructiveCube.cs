using UnityEngine;

public class DestructiveCube : MonoBehaviour
{
    [Header("Settings")]
    public float damageToHand = 20f;

    // This handles objects set as "Trigger"
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleImpact(other.gameObject);
    }

    // This handles objects set as "Solid Collision"
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleImpact(collision.gameObject);
    }

    private void HandleImpact(GameObject hitObject)
    {
        // 1. Check for Player
        if (hitObject.CompareTag("Player"))
        {
            Debug.Log("DESTRUCTIVE CUBE: Hit Player!");
            PlayerController player = hitObject.GetComponentInParent<PlayerController>();
            
            if (player != null)
            {
                player.Die();
            }
            else
            {
                Debug.LogError("DESTRUCTIVE CUBE: Found Player tag, but NO PlayerController script!");
            }
        }

        // 2. Check for Boss
        if (hitObject.CompareTag("Boss"))
        {
            Debug.Log("DESTRUCTIVE CUBE: Hit Boss!");
            HandHealth hand = hitObject.GetComponentInParent<HandHealth>();
            if (hand != null)
            {
                hand.TakeDamage(damageToHand);
            }
        }

        // 3. Check for Platform <--- ADDED THIS
        if (hitObject.CompareTag("Platform"))
        {
            Debug.Log("DESTRUCTIVE CUBE: Destroyed a Platform!");
            Destroy(hitObject); // This destroys the platform that was hit
            
            // Optional: If you want the cube itself to disappear after hitting a platform, 
            // uncomment the line below:
            // Destroy(gameObject); 
        }
    }
}