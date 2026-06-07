using UnityEngine;

public class DestructiveCube : MonoBehaviour
{
    [Header("Settings")]
    public float damageToHand = 20f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check for Player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // We call Die() on the player so that the music stops 
                // and the death sound plays correctly.
                player.Die();
            }
        }

        // 2. Check for Boss Hand (Optional - based on your variables)
        if (other.CompareTag("Boss")) // Make sure your Boss Hand has this tag
        {
            HandHealth hand = other.GetComponent<HandHealth>();
            if (hand != null)
            {
                hand.TakeDamage(damageToHand);
            }
        }

        // 3. Handle Platforms (Destroy them if they touch the cube)
        if (other.CompareTag("Platform"))
        {
            Destroy(other.gameObject);
        }
    }
}