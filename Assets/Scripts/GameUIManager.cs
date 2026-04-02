using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject deathScreen;
    public GameObject winScreen;
    public GameObject pauseMenu;

    private bool isPaused = false;
    private bool gameIsActive = false;

    void Start()
    {
        deathScreen.SetActive(false);
        winScreen.SetActive(false);
        pauseMenu.SetActive(false);
        gameIsActive = true;
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
        gameIsActive = false;
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowWinScreen()
    {
        gameIsActive = false;
        winScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        if (!gameIsActive) return;
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

    public void OnPauseButton()
    {
        TogglePause();
    }
}