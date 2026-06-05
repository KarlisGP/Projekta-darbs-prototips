using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
    [Header("Damage")]
    public float damageAmount = 10f;

    [Header("Movement")]
    public float flyUpForce = 5f;

    [Header("Spin")]
    public float spinSpeed = 360f;

    private bool hasBeenCollected = false;
    private ItemSpawner spawner;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Spin only after pickup
        if (hasBeenCollected)
        {
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenCollected) return;

        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void SetSpawner(ItemSpawner s)
    {
        spawner = s;
    }

    private void Collect()
    {
        hasBeenCollected = true;

        // Damage the hand once
        HandHealth hand = FindObjectOfType<HandHealth>();

        if (hand != null)
        {
            hand.TakeDamage(damageAmount);
        }

        // Stop physics motion
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;

            // Fly upward
            rb.AddForce(Vector2.up * flyUpForce, ForceMode2D.Impulse);
        }

        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (spawner != null)
        {
            spawner.NotifyItemDestroyed();
        }

        Destroy(gameObject);
    }
}