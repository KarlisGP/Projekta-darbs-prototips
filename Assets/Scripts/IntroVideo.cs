using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (PlayerPrefs.GetInt("IntroPlayed", 0) == 1)
        {
            // Already seen it, skip straight to menu
            LoadMenu();
            return;
        }

        StartCoroutine(PlayThenLoad());
    }

    IEnumerator PlayThenLoad()
    {
        videoPlayer.Play();

        yield return new WaitUntil(() => videoPlayer.isPlaying);
        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        LoadMenu();
    }

    void Update()
    {
        if (Input.anyKeyDown)
            LoadMenu();
    }

    void LoadMenu()
    {
        StopAllCoroutines();
        PlayerPrefs.SetInt("IntroPlayed", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }
}