using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DestructiveCube : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject gameOverScreen; // Drag a UI panel into this slot

    private static bool gameIsOver = false;

    void Start()
    {
        gameIsOver = false;
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContact(collision.gameObject);
    }

    private void HandleContact(GameObject other)
    {
        if (other.CompareTag("Player") && !gameIsOver)
        {
            gameIsOver = true;
            GameOver(other.gameObject);
        }

        if (other.CompareTag("Platform"))
        {
            Destroy(other.gameObject);
        }
    }

    private void GameOver(GameObject player)
    {
        // Freeze the player in place
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Show game over screen if assigned
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
        else
            Invoke("ReloadScene", 1f); // fallback: just reload
    }

    // Hook this up to a Retry button in your UI
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