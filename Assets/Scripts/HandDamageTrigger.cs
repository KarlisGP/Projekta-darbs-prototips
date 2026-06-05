using UnityEngine;
using System.Collections;

public class HandDamageTrigger : MonoBehaviour
{
    [Header("Target")]
    public HandHealth targetHand;

    [Header("Damage")]
    public float damageAmount = 25f;

    [Header("Cooldown")]
    public float hitCooldown = 1f;

    private bool canHit = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!canHit)
            return;

        if (targetHand != null)
        {
            targetHand.TakeDamage(damageAmount);
            Debug.Log($"Hand damaged for {damageAmount}");
        }

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        canHit = false;

        yield return new WaitForSeconds(hitCooldown);

        canHit = true;
    }
}