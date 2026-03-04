using UnityEngine;
using UnityEngine.SceneManagement;

public class DestructiveCube : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject gameOverScreen;

    private static bool gameIsOver = false;

    void Start()
    {
        gameIsOver = false;
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other.gameObject);
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
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
        else
            Invoke("ReloadScene", 1f);
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