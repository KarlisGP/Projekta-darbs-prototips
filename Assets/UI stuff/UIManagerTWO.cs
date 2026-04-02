using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerTWO : MonoBehaviour
{
    [Header("Screens")]
    public GameObject startScreen;
    public GameObject deathScreen;
    public GameObject pauseMenu;
    public GameObject winScreen;
    public GameObject settingsScreen;

    [Header("Audio")]
    public AudioSource menuMusic;

    [Header("Mute Button")]
    public Sprite muteSprite;
    public Sprite unmuteSprite;
    public Image muteButtonImage;

    private bool isPaused = false;

    void Start()
    {
        ShowStartScreen();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // --- Start Screen ---
    public void ShowStartScreen()
    {
        startScreen.SetActive(true);
        deathScreen.SetActive(false);
        pauseMenu.SetActive(false);
        winScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }

    public void OnStartButton()
    {
        startScreen.SetActive(false);
        settingsScreen.SetActive(false);
        menuMusic.Stop();
        Time.timeScale = 1f;
    }

    // --- Death & Win ---
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

    // --- Shared Buttons ---
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

    public void OnSettingsButton()
    {
        settingsScreen.SetActive(true);
    }

    public void OnSettingsBackButton()
    {
        settingsScreen.SetActive(false);
    }

    // --- Pause ---
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

    public void OnQuitButton()
    {
        Application.Quit();
    }

    // --- Mute ---
    public void OnMuteButton()
    {
        menuMusic.mute = !menuMusic.mute;
        muteButtonImage.sprite = menuMusic.mute ? muteSprite : unmuteSprite;
    }
}