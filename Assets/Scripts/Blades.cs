using UnityEngine;
using UnityEngine.SceneManagement;

public class Blades : MonoBehaviour
{
    private GameUIManager uiManager;

    [Header("Hand Damage")]
    public float handDamage = 25f;

    void Start()
    {
        uiManager = FindObjectOfType<GameUIManager>();
    }

    // This handles triggers
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other.gameObject);
    }

    // This handles solid collisions (just in case)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContact(collision.gameObject);
    }

    private void HandleContact(GameObject other)
    {
        // 1. DAMAGE GIANT HAND
        HandHealth hand = other.GetComponentInParent<HandHealth>();
        if (hand != null)
        {
            hand.TakeDamage(handDamage);
            return; // Exit so we don't try to kill the hand as a player
        }

        // 2. KILL PLAYER
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                // CRITICAL: We call the Player's Die() function.
                // This is the function that stops the boss music and plays the death sound.
                player.Die();
            }
        }

        // 3. DESTROY FALLING PLATFORMS
        if (other.CompareTag("Platform"))
        {
            Destroy(other.gameObject);
        }
    }

    // This remains for your UI buttons to call
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}