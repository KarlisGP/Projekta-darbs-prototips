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

    private bool isPaused = false;
    private int currentSlide = 0;

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

    // --- Helper ---
    void PlayClick()
    {
        sfxSource.PlayOneShot(buttonClick);
    }

    // --- Start Screen ---
    public void ShowStartScreen()
    {
        startScreen.SetActive(true);
        deathScreen.SetActive(false);
        pauseMenu.SetActive(false);
        winScreen.SetActive(false);
        settingsScreen.SetActive(false);
        tutorialScreen.SetActive(false);
    }

    public void OnStartButton()
    {
        Debug.Log("Start button pressed");
        Debug.Log("tutorialScreen: " + tutorialScreen);
        Debug.Log("tutorialSlides length: " + tutorialSlides.Length);
        Debug.Log("tutorialImage: " + tutorialImage);
    
        PlayClick();
        menuMusic.Stop();
        startScreen.SetActive(false);
        currentSlide = 0;
        tutorialScreen.SetActive(true);
        Debug.Log("tutorialScreen active: " + tutorialScreen.activeSelf);
        tutorialImage.sprite = tutorialSlides[currentSlide];
        Debug.Log("Slide set to: " + tutorialSlides[currentSlide].name);
    }

    // --- Tutorial ---
    public void OnTutorialNextButton()
    {
        PlayClick();
        currentSlide++;

        if (currentSlide >= tutorialSlides.Length)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(1);
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
        SceneManager.LoadScene(1);
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
        PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuButton()
    {
        PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
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

    // --- Pause ---
    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnResumeButton()
    {
        PlayClick();
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnQuitButton()
    {
        PlayClick();
        Application.Quit();
    }

    // --- Mute ---
    public void OnMuteButton()
    {
        PlayClick();
        menuMusic.mute = !menuMusic.mute;
        muteButtonImage.sprite = menuMusic.mute ? muteSprite : unmuteSprite;
    }
}