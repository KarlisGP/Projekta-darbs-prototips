using UnityEngine;
using UnityEngine.SceneManagement;

public class DestructiveCube : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContact(collision.gameObject);
    }

    private void HandleContact(GameObject other)
    {
        // Kill player = reload the scene (game over)
        if (other.CompareTag("Player"))
        {
            GameOver();
        }

        // Destroy platforms on contact
        if (other.CompareTag("Platform"))
        {
            Destroy(other.gameObject);
        }
    }

    private void GameOver()
    {
        // Small delay feels better than instant reload
        Invoke("ReloadScene", 0.5f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}