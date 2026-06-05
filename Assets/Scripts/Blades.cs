using UnityEngine;
using UnityEngine.SceneManagement;

public class Blades : MonoBehaviour
{
    private static bool gameIsOver = false;
    private GameUIManager uiManager;

    [Header("Hand Damage")]
    public float handDamage = 25f;

    void Start()
    {
        gameIsOver = false;
        uiManager = FindObjectOfType<GameUIManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other.gameObject);
    }

    private void HandleContact(GameObject other)
    {
        // Damage giant hand
        HandHealth hand = other.GetComponent<HandHealth>();

        if (hand != null)
        {
            hand.TakeDamage(handDamage);
            return;
        }

        // Kill player
        if (other.CompareTag("Player") && !gameIsOver)
        {
            gameIsOver = true;
            GameOver(other.gameObject);
        }

        // Destroy falling platforms
        if (other.CompareTag("Platform"))
        {
            Destroy(other.gameObject);
        }
    }

    private void GameOver(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (uiManager != null)
            uiManager.ShowDeathScreen();
        else
            Invoke(nameof(ReloadScene), 1f);
    }

    public void Retry()
    {
        gameIsOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}