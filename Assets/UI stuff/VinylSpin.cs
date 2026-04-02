using UnityEngine;

public class VinylSpin : MonoBehaviour
{
    public float speed = 45f; // degrees per second
    public AudioSource audioSource;

    void Update()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            transform.Rotate(0f, 0f, -speed * Time.unscaledDeltaTime);
        }
    }
}