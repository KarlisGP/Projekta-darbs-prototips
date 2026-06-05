using UnityEngine;

public class HandHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Effects")]
    public bool destroyOnDeath = true;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Hand spawned with {currentHealth} HP");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log($"Hand took {damage} damage. Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("HAND DESTROYED!");

        if (destroyOnDeath)
            Destroy(gameObject);
    }
}