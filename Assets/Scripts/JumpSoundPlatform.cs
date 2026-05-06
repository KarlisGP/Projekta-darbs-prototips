using UnityEngine;

public class JumpSoundPlatform : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip jumpSound;

    // The player will call this function
    public void PlayJumpSound()
    {
        if (audioSource && jumpSound)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }
}
