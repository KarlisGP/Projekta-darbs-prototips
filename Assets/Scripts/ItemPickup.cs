using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
    [Header("Target")]
    public HandHealth targetHand;

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
        if (hasBeenCollected)
        {
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenCollected)
            return;

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

        if (targetHand != null)
        {
            targetHand.TakeDamage(damageAmount);

            Debug.Log(
                $"Pickup dealt {damageAmount} damage. Hand HP now: {targetHand.currentHealth}"
            );
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.AddForce(Vector2.up * flyUpForce, ForceMode2D.Impulse);
        }

        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.4f);

        if (spawner != null)
        {
            spawner.NotifyItemDestroyed();
        }

        Destroy(gameObject);
    }
}