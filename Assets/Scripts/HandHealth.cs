using UnityEngine;

public class HandHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Hand Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Hand Destroyed!");

        // Add your game over logic here
        // Example:
        // FindObjectOfType<GameUIManager>().ShowDeathScreen();

        Destroy(gameObject);
    }
}