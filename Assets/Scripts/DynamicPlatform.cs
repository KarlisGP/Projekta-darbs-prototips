using UnityEngine;
using System.Collections;

public class DynamicPlatform : MonoBehaviour
{
    public float switchInterval = 2f;
    public float respawnDelay = 1.5f;
    [SerializeField] private float downwardForce = 5f; // The "kick" to prevent jumping from Red

    private bool isJumpMode = true;

    private EdgeCollider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        col = GetComponent<EdgeCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        StartCoroutine(SwitchMode());
        UpdateVisual();
    }

    IEnumerator SwitchMode()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchInterval);
            isJumpMode = !isJumpMode;
            UpdateVisual();
        }
    }

    void UpdateVisual()
    {
        if (sr != null)
            sr.color = isJumpMode ? Color.green : Color.red;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (isJumpMode)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.GiveExtraJump(1);
            }
        }
        else // RED MODE
        {
            if (playerRb != null)
            {
                // 1. Immediately kill any upward velocity the player has
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, -1f);

                // 2. Apply a downward push (using transform.up * -1 handles the rotation)
                // This pushes the player "away" from the platform's surface
                playerRb.AddForce(-transform.up * downwardForce, ForceMode2D.Impulse);
            }

            // 3. Start the disappearing routine
            StartCoroutine(DisablePlatform());
        }
    }

    IEnumerator DisablePlatform()
    {
        // Smallest possible delay so the physics engine processes the 'push' 
        // before the collider vanishes
        yield return new WaitForFixedUpdate(); 
        
        col.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        col.enabled = true;
    }
}