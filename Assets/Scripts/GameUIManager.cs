using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    IEnumerator LoadWithDelay(int sceneIndex)
    {
        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene(sceneIndex);
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

    public void OnNextLevelButton()
    {
        Time.timeScale = 1f;
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene > 5)
            StartCoroutine(LoadWithDelay(0));
        else
            StartCoroutine(LoadWithDelay(nextScene));
    }

    public void OnBackButton()
    {
        Time.timeScale = 1f;
        int previousScene = SceneManager.GetActiveScene().buildIndex - 1;
        if (previousScene < 0) previousScene = 0;
        StartCoroutine(LoadWithDelay(previousScene));
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadWithDelay(SceneManager.GetActiveScene().buildIndex));
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadWithDelay(0));
    }

    public void OnPauseButton()
    {
        TogglePause();
    }
}