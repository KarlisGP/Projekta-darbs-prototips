using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject deathScreen;
    public GameObject winScreen;
    public GameObject pauseMenu;

    [Header("Level Tutorial")]
    public GameObject tutorialPanel;
    public Image tutorialSlideImage;
    public Sprite[] levelTutorialSlides;
    public GameObject tutorialPrevButton;
    public GameObject tutorialNextButton;
    public GameObject tutorialPlayButton;

    private int currentSlide = 0;
    private bool isPaused = false;
    private bool gameIsActive = false;

    void Start()
    {
        deathScreen.SetActive(false);
        winScreen.SetActive(false);
        pauseMenu.SetActive(false);

        // Hide all tutorial buttons by default
        if (tutorialPrevButton != null) tutorialPrevButton.SetActive(false);
        if (tutorialNextButton != null) tutorialNextButton.SetActive(false);
        if (tutorialPlayButton != null) tutorialPlayButton.SetActive(false);

        if (levelTutorialSlides != null && levelTutorialSlides.Length > 0)
        {
            Time.timeScale = 0f;
            currentSlide = 0;
            ShowTutorialSlide();
            tutorialPanel.SetActive(true);
        }
        else
        {
            tutorialPanel.SetActive(false);
            gameIsActive = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    // --- Level Tutorial ---

    void ShowTutorialSlide()
    {
        tutorialSlideImage.sprite = levelTutorialSlides[currentSlide];

        bool isFirst = currentSlide == 0;
        bool isLast = currentSlide == levelTutorialSlides.Length - 1;

        if (tutorialPrevButton != null) tutorialPrevButton.SetActive(!isFirst);
        if (tutorialNextButton != null) tutorialNextButton.SetActive(!isLast);
        if (tutorialPlayButton != null) tutorialPlayButton.SetActive(isLast);
    }

    public void OnTutorialNext()
    {
        if (currentSlide < levelTutorialSlides.Length - 1)
        {
            currentSlide++;
            ShowTutorialSlide();
        }
    }

    public void OnTutorialPrev()
    {
        if (currentSlide > 0)
        {
            currentSlide--;
            ShowTutorialSlide();
        }
    }

    public void OnTutorialSkip()
    {
        DismissTutorial();
    }

    public void OnTutorialPlay()
    {
        DismissTutorial();
    }

    void DismissTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        gameIsActive = true;
    }

    // --- Screens ---

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
        if (nextScene > 11)
            StartCoroutine(LoadWithDelay(1));
        else
            StartCoroutine(LoadWithDelay(nextScene));
    }

    public void OnBackButton()
    {
        Time.timeScale = 1f;
        int previousScene = SceneManager.GetActiveScene().buildIndex - 1;
        if (previousScene < 1) previousScene = 1;
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
        StartCoroutine(LoadWithDelay(1));
    }

    public void OnPauseButton()
    {
        TogglePause();
    }
}