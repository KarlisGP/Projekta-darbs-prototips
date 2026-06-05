using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Destination")]
    public Transform teleportExit; // Drag the 'ExitPoint' object here

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Move the player to the exit point
            other.transform.position = teleportExit.position;

            // 2. Reset player velocity so they don't 'fly' after teleporting
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            Debug.Log("Player Teleported to the Boss Platform!");
        }
    }
}