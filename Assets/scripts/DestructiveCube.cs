using UnityEngine;
using UnityEngine.SceneManagement;

public class DestructiveCube : MonoBehaviour
{
    public int damageToHand = 20;
    private HandHealth myHealth;
    private GameUIManager uiManager;

    void Start()
    {
        myHealth = GetComponent<HandHealth>();
        uiManager = FindObjectOfType<GameUIManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kill Player
        if (other.CompareTag("Player"))
        {
            if (uiManager != null) uiManager.ShowDeathScreen();
            else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // 2. Handle Platforms
        if (other.CompareTag("Platform"))
        {
            // If the platform is an Ice cube, damage the hand
            // We check the name for "Ice" (Case sensitive: "Ice")
            if (other.gameObject.name.Contains("Ice"))
            {
                if (myHealth != null)
                {
                    myHealth.TakeDamage(damageToHand);
                }
            }

            // Destroy the platform regardless of what it is
            Destroy(other.gameObject);
        }
    }
}