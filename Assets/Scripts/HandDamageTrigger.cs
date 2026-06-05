using UnityEngine;
using System.Collections;

public class HandDamageTrigger : MonoBehaviour
{
    [Header("Target")]
    public HandHealth hand;

    [Header("Damage")]
    public float damageAmount = 25f;

    [Header("Cooldown")]
    public float hitCooldown = 1f;

    private bool canHit = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        TryDamageHand();
    }

    private void TryDamageHand()
    {
        if (!canHit) return;

        if (hand == null)
        {
            hand = FindObjectOfType<HandHealth>();
        }

        if (hand != null)
        {
            hand.TakeDamage(damageAmount);
            Debug.Log($"Trigger hit → Hand takes {damageAmount} damage");
        }
        else
        {
            Debug.LogWarning("No HandHealth found in scene!");
        }

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        canHit = false;
        yield return new WaitForSeconds(hitCooldown);
        canHit = true;

        Debug.Log("Hand trigger cooldown ready");
    }
}