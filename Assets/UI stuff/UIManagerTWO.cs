using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManagerTWO : MonoBehaviour
{
    [Header("Screens")]
    public GameObject startScreen;
    public GameObject deathScreen;
    public GameObject winScreen;
    public GameObject settingsScreen;
    public GameObject levelSelectorScreen;
    public GameObject pauseMenu;

    [Header("Tutorial")]
    public GameObject tutorialScreen;
    public Image tutorialImage;
    public Sprite[] tutorialSlides;

    [Header("Audio")]
    public AudioSource menuMusic;

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip buttonClick;

    [Header("Mute Button")]
    public Sprite muteSprite;
    public Sprite unmuteSprite;
    public Image muteButtonImage;

    private int currentSlide = 0;
    private bool isPaused = false;
    private bool gameIsActive = false;

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

    void PlayClick()
    {
        sfxSource.PlayOneShot(buttonClick);
    }

    IEnumerator LoadWithDelay(int sceneIndex)
    {
        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene(sceneIndex);
    }

    public void ShowStartScreen()
    {
        startScreen.SetActive(true);
        deathScreen.SetActive(false);
        winScreen.SetActive(false);
        settingsScreen.SetActive(false);
        tutorialScreen.SetActive(false);
        levelSelectorScreen.SetActive(false);
        pauseMenu.SetActive(false);
        gameIsActive = false;
    }

    // --- Start Button ---
    public void OnStartButton()
    {
        PlayClick();
        menuMusic.Stop();
        startScreen.SetActive(false);
        levelSelectorScreen.SetActive(true);
    }

    // --- Level Selector ---
    public void LoadLevel1() { PlayClick(); StartCoroutine(LoadWithDelay(1)); }
    public void LoadLevel2() { PlayClick(); StartCoroutine(LoadWithDelay(2)); }
    public void LoadLevel3() { PlayClick(); StartCoroutine(LoadWithDelay(3)); }
    public void LoadLevel4() { PlayClick(); StartCoroutine(LoadWithDelay(4)); }
    public void LoadLevel5() { PlayClick(); StartCoroutine(LoadWithDelay(5)); }

    public void OnLevelSelectorBackButton()
    {
        PlayClick();
        levelSelectorScreen.SetActive(false);
        startScreen.SetActive(true);
        menuMusic.Play();
    }

    // --- Tutorial ---
    public void OnTutorialNextButton()
    {
        PlayClick();
        currentSlide++;

        if (currentSlide >= tutorialSlides.Length)
        {
            Time.timeScale = 1f;
            StartCoroutine(LoadWithDelay(1));
        }
        else
        {
            tutorialImage.sprite = tutorialSlides[currentSlide];
        }
    }

    public void OnTutorialSkipButton()
    {
        PlayClick();
        Time.timeScale = 1f;
        StartCoroutine(LoadWithDelay(1));
    }

    // --- Death & Win ---
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

    // --- Pause ---
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

    public void OnPauseButton()
    {
        TogglePause();
    }

    // --- Shared Buttons ---
    public void OnRetryButton()
    {
        PlayClick();
        Time.timeScale = 1f;
        StartCoroutine(LoadWithDelay(SceneManager.GetActiveScene().buildIndex));
    }

    public void OnMainMenuButton()
    {
        PlayClick();
        Time.timeScale = 1f;
        StartCoroutine(LoadWithDelay(0));
    }

    public void OnBackButton()
    {
        PlayClick();
        Time.timeScale = 1f;
        int previousScene = SceneManager.GetActiveScene().buildIndex - 1;
        if (previousScene < 0) previousScene = 0;
        StartCoroutine(LoadWithDelay(previousScene));
    }

    public void OnSettingsButton()
    {
        PlayClick();
        settingsScreen.SetActive(true);
    }

    public void OnSettingsBackButton()
    {
        PlayClick();
        settingsScreen.SetActive(false);
    }

    public void OnQuitButton()
    {
        PlayClick();
        Application.Quit();
    }

    public void OnMuteButton()
    {
        PlayClick();
        menuMusic.mute = !menuMusic.mute;
        muteButtonImage.sprite = menuMusic.mute ? muteSprite : unmuteSprite;
    }
}