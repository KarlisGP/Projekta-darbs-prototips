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
    public GameObject loadingScreen;
    public GameObject tutorialBrowserScreen;

    [Header("Loading Overlay")]
    public RectTransform loadingOverlay;
    public RectTransform whirlpoolImage;
    public float overlayZoomDuration = 0.6f;
    public float whirlpoolSpinSpeed = 120f;

    [Header("Tutorial (In-Game)")]
    public GameObject tutorialScreen;
    public Image tutorialImage;
    public Sprite[] tutorialSlides;

    [Header("Tutorial Browser (Settings)")]
    public Image browserTutorialImage;
    public Sprite[] browserTutorialSlides;
    public GameObject browserPrevButton;
    public GameObject browserNextButton;

    [Header("Audio")]
    public AudioSource menuMusic;

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip buttonClick;

    [Header("Mute Button")]
    public Sprite muteSprite;
    public Sprite unmuteSprite;
    public Image muteButtonImage;

    [Header("Menu Animation")]
    public RectTransform blade1;
    public RectTransform blade2;
    public RectTransform blade3;
    public RectTransform blade4;
    public RectTransform fleaOnBerry;

    public float blade1Speed = 105f;
    public float blade2Speed = 90f;
    public float blade3Speed = 75f;
    public float blade4Speed = 60f;
    public float fleaSpinSpeed = 0.5f;

    public float blade2Delay = 0.3f;
    public float blade3Delay = 0.6f;
    public float blade4Delay = 1.0f;

    public float spinTransitionDuration = 2.5f;
    public float spinUpDuration = 0.8f;
    public float spinUpPeakMultiplier = 5f;

    private bool blade1Spinning = false;
    private bool blade2Spinning = false;
    private bool blade3Spinning = false;
    private bool blade4Spinning = false;

    private float spinMultiplier = 1f;
    private float speedBoost = 1f;

    private int currentSlide = 0;
    private int browserSlide = 0;
    private bool isPaused = false;
    private bool gameIsActive = false;

    void Start()
    {
        if (loadingOverlay != null)
            loadingOverlay.localScale = Vector3.zero;

        ShowStartScreen();
        StartCoroutine(StartBladesSequence());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();

        float finalMultiplier = spinMultiplier * speedBoost;

        if (blade1Spinning && blade1 != null)
            blade1.Rotate(0f, 0f, -blade1Speed * finalMultiplier * Time.deltaTime);

        if (blade2Spinning && blade2 != null)
            blade2.Rotate(0f, 0f, -blade2Speed * finalMultiplier * Time.deltaTime);

        if (blade3Spinning && blade3 != null)
            blade3.Rotate(0f, 0f, -blade3Speed * finalMultiplier * Time.deltaTime);

        if (blade4Spinning && blade4 != null)
            blade4.Rotate(0f, 0f, -blade4Speed * finalMultiplier * Time.deltaTime);

        if (fleaOnBerry != null)
            fleaOnBerry.Rotate(0f, 0f, -fleaSpinSpeed * finalMultiplier * Time.deltaTime);

        if (whirlpoolImage != null)
            whirlpoolImage.Rotate(0f, 0f, -whirlpoolSpinSpeed * Time.deltaTime);
    }

    IEnumerator StartBladesSequence()
    {
        blade1Spinning = true;

        yield return new WaitForSeconds(blade2Delay);
        blade2Spinning = true;

        yield return new WaitForSeconds(blade3Delay - blade2Delay);
        blade3Spinning = true;

        yield return new WaitForSeconds(blade4Delay - blade3Delay);
        blade4Spinning = true;
    }

    IEnumerator TransitionSpin(float targetMultiplier)
    {
        float startMultiplier = spinMultiplier;
        float elapsed = 0f;
        while (elapsed < spinTransitionDuration)
        {
            elapsed += Time.deltaTime;
            spinMultiplier = Mathf.Lerp(startMultiplier, targetMultiplier, elapsed / spinTransitionDuration);
            yield return null;
        }
        spinMultiplier = targetMultiplier;
    }

    IEnumerator SpinUpThenLoad(int sceneIndex)
    {
        float elapsed = 0f;
        float totalDuration = Mathf.Max(spinUpDuration, overlayZoomDuration);

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            if (elapsed <= spinUpDuration)
                speedBoost = Mathf.Lerp(1f, spinUpPeakMultiplier, elapsed / spinUpDuration);

            if (loadingOverlay != null)
            {
                float overlayT = Mathf.SmoothStep(0f, 1f, elapsed / overlayZoomDuration);
                float scale = Mathf.Lerp(0f, 1.5f, overlayT);
                loadingOverlay.localScale = new Vector3(scale, scale, 1f);
            }

            yield return null;
        }

        menuMusic.Stop();
        SceneManager.LoadScene(sceneIndex);
    }

    // --- Tutorial Browser ---

    void UpdateBrowserSlide()
    {
        if (browserTutorialImage != null && browserTutorialSlides.Length > 0)
            browserTutorialImage.sprite = browserTutorialSlides[browserSlide];

        if (browserPrevButton != null)
            browserPrevButton.SetActive(browserSlide > 0);

        if (browserNextButton != null)
            browserNextButton.SetActive(browserSlide < browserTutorialSlides.Length - 1);
    }

    public void OnBrowserNextButton()
    {
        PlayClick();
        if (browserSlide < browserTutorialSlides.Length - 1)
        {
            browserSlide++;
            UpdateBrowserSlide();
        }
    }

    public void OnBrowserPrevButton()
    {
        PlayClick();
        if (browserSlide > 0)
        {
            browserSlide--;
            UpdateBrowserSlide();
        }
    }

    public void OnOpenTutorialBrowser()
    {
        PlayClick();
        browserSlide = 0;
        UpdateBrowserSlide();
        settingsScreen.SetActive(false);
        tutorialBrowserScreen.SetActive(true);
    }

    public void OnCloseTutorialBrowser()
    {
        PlayClick();
        tutorialBrowserScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }

    // ---- Everything else ----

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
        tutorialBrowserScreen.SetActive(false);
        levelSelectorScreen.SetActive(false);
        pauseMenu.SetActive(false);
        if (loadingScreen != null) loadingScreen.SetActive(false);
        gameIsActive = false;
    }

    public void OnStartButton()
    {
        PlayClick();
        startScreen.SetActive(false);
        levelSelectorScreen.SetActive(true);
    }

    // Levels 1-10: scene indices 2-11
    public void LoadLevel1()  { PlayClick(); StartCoroutine(SpinUpThenLoad(2));  }
    public void LoadLevel2()  { PlayClick(); StartCoroutine(SpinUpThenLoad(3));  }
    public void LoadLevel3()  { PlayClick(); StartCoroutine(SpinUpThenLoad(4));  }
    public void LoadLevel4()  { PlayClick(); StartCoroutine(SpinUpThenLoad(5));  }
    public void LoadLevel5()  { PlayClick(); StartCoroutine(SpinUpThenLoad(6));  }
    public void LoadLevel6()  { PlayClick(); StartCoroutine(SpinUpThenLoad(7));  }
    public void LoadLevel7()  { PlayClick(); StartCoroutine(SpinUpThenLoad(8));  }
    public void LoadLevel8()  { PlayClick(); StartCoroutine(SpinUpThenLoad(9));  }
    public void LoadLevel9()  { PlayClick(); StartCoroutine(SpinUpThenLoad(10)); }
    public void LoadLevel10() { PlayClick(); StartCoroutine(SpinUpThenLoad(11)); }

    public void OnLevelSelectorBackButton()
    {
        PlayClick();
        levelSelectorScreen.SetActive(false);
        startScreen.SetActive(true);
    }

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

    public void OnPauseButton() { TogglePause(); }

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
        StartCoroutine(LoadWithDelay(1));
    }

    public void OnBackButton()
    {
        PlayClick();
        Time.timeScale = 1f;
        int previousScene = SceneManager.GetActiveScene().buildIndex - 1;
        if (previousScene < 0) previousScene = 0;
        StartCoroutine(LoadWithDelay(previousScene));
    }

    public void OnSettingsButton() { PlayClick(); settingsScreen.SetActive(true); }
    public void OnSettingsBackButton() { PlayClick(); settingsScreen.SetActive(false); }
    public void OnQuitButton() { PlayClick(); Application.Quit(); }

    public void OnMuteButton()
    {
        PlayClick();
        menuMusic.mute = !menuMusic.mute;
        muteButtonImage.sprite = menuMusic.mute ? muteSprite : unmuteSprite;

        StopCoroutine("TransitionSpin");
        StartCoroutine(TransitionSpin(menuMusic.mute ? 0f : 1f));
    }
}