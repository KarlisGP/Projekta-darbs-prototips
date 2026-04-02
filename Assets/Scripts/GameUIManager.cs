using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject deathScreen;
    public GameObject winScreen;
    public GameObject pauseMenu;

    private bool isPaused = false;

    void Start()
    {
        deathScreen.SetActive(false);
        winScreen.SetActive(false);
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnResumeButton()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}