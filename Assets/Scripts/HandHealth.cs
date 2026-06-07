using UnityEngine;

public class HandHealth : MonoBehaviour
{
    [Header("References")]
    public BossArmAI armAI;
    public GameUIManager uiManager;

    [Header("Health")]
    public float maxHealth = 10f;
    public float currentHealth;
    private bool isDead = false;

    [Header("Hand Sprites (6 total)")]
    public SpriteRenderer handRenderer;
    public Sprite upSprite1;
    public Sprite upSprite2;
    public Sprite upSprite3;
    public Sprite downSprite1;
    public Sprite downSprite2;
    public Sprite downSprite3;

    [Header("Elbow Sprites (3 total)")]
    public SpriteRenderer elbowRenderer;
    public Sprite elbow1;
    public Sprite elbow2;
    public Sprite elbow3;

    [Header("Health Meter Sprite")]
    public Transform healthSprite;
    public float fullSize = 1f;
    public float minimumSize = 0f;

    [Header("Effects")]
    public AudioClip damageSound;
    public AudioClip deathSound;

    private AudioSource audioSource;
    private EyeTracker eyeTracker;

    private void Start()
    {
        currentHealth = maxHealth;

        if (handRenderer == null)
            handRenderer = GetComponent<SpriteRenderer>();

        if (armAI == null)
            armAI = GetComponentInParent<BossArmAI>();

        if (uiManager == null)
            uiManager = FindObjectOfType<GameUIManager>();

        eyeTracker = FindObjectOfType<EyeTracker>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        UpdateVisuals();
        UpdateHealthSprite();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        Debug.Log($"Boss HP: {currentHealth}/{maxHealth}");

        if (damageSound != null)
            audioSource.PlayOneShot(damageSound);

        if (eyeTracker != null)
            eyeTracker.TriggerJitter();

        UpdateVisuals();
        UpdateHealthSprite();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateVisuals()
    {
        if (armAI == null)
            return;

        float healthPercent = currentHealth / maxHealth;

        int stage;

        if (healthPercent > 0.66f)
            stage = 1;
        else if (healthPercent > 0.33f)
            stage = 2;
        else
            stage = 3;

        bool isSlamming = armAI.currentState == BossArmAI.State.Slam;

        // Hand sprites
        if (handRenderer != null)
        {
            if (isSlamming)
            {
                handRenderer.sprite =
                    stage == 1 ? downSprite1 :
                    stage == 2 ? downSprite2 :
                    downSprite3;
            }
            else
            {
                handRenderer.sprite =
                    stage == 1 ? upSprite1 :
                    stage == 2 ? upSprite2 :
                    upSprite3;
            }
        }

        // Elbow sprites
        if (elbowRenderer != null)
        {
            elbowRenderer.sprite =
                stage == 1 ? elbow1 :
                stage == 2 ? elbow2 :
                elbow3;
        }
    }

    private void UpdateHealthSprite()
    {
        if (healthSprite == null)
            return;

        float healthPercent = currentHealth / maxHealth;

        float scale = Mathf.Lerp(minimumSize, fullSize, healthPercent);

        healthSprite.localScale = new Vector3(
            scale,
            scale,
            healthSprite.localScale.z
        );
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Stop all currently playing audio
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            source.Stop();
        }

        // Play death sound
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(
                deathSound,
                Camera.main != null ? Camera.main.transform.position : transform.position
            );
        }

        Debug.Log("Boss Defeated!");

        if (uiManager != null)
        {
            uiManager.ShowWinScreen();
        }
    }
}