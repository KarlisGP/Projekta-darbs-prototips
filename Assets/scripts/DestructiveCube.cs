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
            if (uiManager != null) 
            {
                uiManager.ShowDeathScreen();
            }
            else 
            {
                // Fallback if uiManager isn't found
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        // 2. Handle Platforms
        if (other.CompareTag("Platform"))
        { // <--- This was missing
            Destroy(other.gameObject);
        } // <--- This was closing the method early because of the missing one above
    }
}