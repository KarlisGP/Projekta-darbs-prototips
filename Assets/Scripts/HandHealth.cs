using UnityEngine;

public class HandHealth : MonoBehaviour
{
    [Header("References")]
    public BossArmAI armAI;
    public GameUIManager uiManager;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    private bool isDead = false;

    [Header("Audio")]
    public AudioClip damageSound;
    public AudioClip deathSound;

    [Header("Sprites (Damaged versions)")]
    public Sprite downSprite1;
    public Sprite downSprite2;
    public Sprite downSprite3;

    public Sprite upSprite1;
    public Sprite upSprite2;
    public Sprite upSprite3;

    private SpriteRenderer spriteRenderer;
    private EyeTracker eyeTracker;
    private AudioSource audioSource;

    private void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (armAI == null)
            armAI = GetComponentInParent<BossArmAI>();

        if (uiManager == null)
            uiManager = FindObjectOfType<GameUIManager>();

        eyeTracker = FindObjectOfType<EyeTracker>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        UpdateSprite();
    }

    private void Update()
    {
        UpdateSprite();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        // 🔊 Damage sound
        if (damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // 👁 Eye reaction
        if (eyeTracker != null)
        {
            eyeTracker.TriggerJitter();
        }

        Debug.Log($"Hand HP: {currentHealth}");

        UpdateSprite();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        // 🛑 STOP ALL AUDIO IN SCENE
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in allAudio)
        {
            a.Stop();
        }

        // 💀 Play death sound AFTER stopping everything
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        Debug.Log("Boss defeated!");

        if (uiManager != null)
        {
            uiManager.ShowWinScreen();
        }
        else
        {
            Debug.LogError("GameUIManager missing!");
        }
    }

    private void UpdateSprite()
    {
        if (armAI == null || spriteRenderer == null)
            return;

        float healthPercent = currentHealth / maxHealth;

        int stage =
            (healthPercent > 0.66f) ? 0 :
            (healthPercent > 0.33f) ? 1 : 2;

        bool useDownSprites = (armAI.currentState == BossArmAI.State.Slam);

        Sprite targetSprite = null;

        if (useDownSprites)
        {
            if (stage == 0) targetSprite = downSprite1;
            else if (stage == 1) targetSprite = downSprite2;
            else targetSprite = downSprite3;
        }
        else
        {
            if (stage == 0) targetSprite = upSprite1;
            else if (stage == 1) targetSprite = upSprite2;
            else targetSprite = upSprite3;
        }

        if (targetSprite != null && spriteRenderer.sprite != targetSprite)
        {
            spriteRenderer.sprite = targetSprite;
        }
    }
}