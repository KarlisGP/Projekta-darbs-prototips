using UnityEngine;

public class BouncyTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportExit;
    public float clearRadius = 3f;

    [Header("Audio")]
    public AudioClip teleportSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (teleportExit != null)
            {
                // 🔊 PLAY TELEPORT SOUND
                if (teleportSound != null)
                {
                    audioSource.PlayOneShot(teleportSound);
                }

                // 1. Clear exit area
                ClearExitArea();

                // 2. Teleport player
                other.transform.position = teleportExit.position;

                // 3. Reset physics
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    rb.Sleep();

                    Debug.Log("Player Teleported: All forces and momentum killed.");
                }
            }
        }
    }

    void ClearExitArea()
    {
        if (teleportExit == null) return;

        Collider2D[] collidersAtExit =
            Physics2D.OverlapCircleAll(teleportExit.position, clearRadius);

        foreach (Collider2D col in collidersAtExit)
        {
            if (col.CompareTag("Platform"))
            {
                Destroy(col.gameObject);
            }
        }
    }
}