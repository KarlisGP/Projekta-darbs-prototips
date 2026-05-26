using UnityEngine;
using System.Collections;

public class DynamicPlatform : MonoBehaviour
{
    [Header("Settings")]
    public float switchInterval = 2f;
    [SerializeField] private float downwardForce = 12f;
    [SerializeField] private LayerMask playerLayer; 

    [Header("State (Debug)")]
    [SerializeField] private bool isJumpMode = true; 

    private EdgeCollider2D col; 
    private SpriteRenderer sr;
    private Coroutine stateRoutine;

    void Start()
    {
        col = GetComponent<EdgeCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        // Set initial state
        if (isJumpMode)
        {
            sr.color = Color.green;
            col.enabled = true;
        }
        else
        {
            sr.color = Color.red;
            col.enabled = false;
        }

        StartCoroutine(TimerLoop());
    }

    IEnumerator TimerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchInterval);
            isJumpMode = !isJumpMode;

            if (stateRoutine != null) StopCoroutine(stateRoutine);
            stateRoutine = StartCoroutine(HandleStateChange());
        }
    }

    IEnumerator HandleStateChange()
    {
        if (isJumpMode)
        {
            // --- SWITCHING TO GREEN ---
            sr.color = Color.green;

            // ANTI-STUCK SYSTEM:
            // Instead of checking the line, we check the whole visual area of the sprite
            bool playerInside = true;
            while (playerInside)
            {
                // Get the visual size of the jelly bean
                Bounds spriteBounds = sr.bounds;
                
                // We check a box the exact size of the sprite visual
                // We add a tiny bit of "padding" (0.1f) to be safe
                Collider2D hit = Physics2D.OverlapBox(
                    spriteBounds.center, 
                    spriteBounds.size + new Vector3(0.1f, 0.1f, 0), 
                    transform.eulerAngles.z, // This handles rotated jelly beans!
                    playerLayer
                );
                
                if (hit == null)
                {
                    playerInside = false;
                }
                else
                {
                    // Player is somewhere inside the jelly bean's body. 
                    // DO NOT turn on the collider yet.
                    yield return new WaitForFixedUpdate();
                }
            }
            col.enabled = true;
        }
        else
        {
            // --- SWITCHING TO RED ---
            sr.color = Color.red;
            
            // Give physics a frame to apply the 'kick' force
            yield return new WaitForFixedUpdate(); 
            col.enabled = false; 
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (!isJumpMode)
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, -2f);
                playerRb.AddForce(-transform.up * downwardForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null) player.GiveExtraJump(1);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isJumpMode && collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null) player.GiveExtraJump(1);
        }
    }
}