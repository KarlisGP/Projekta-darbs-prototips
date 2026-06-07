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

    [Header("Hand Sprites (6 total)")]
    public SpriteRenderer handRenderer;
    public Sprite upSprite1, upSprite2, upSprite3;
    public Sprite downSprite1, downSprite2, downSprite3;

    [Header("Elbow Sprites (3 total)")]
    public SpriteRenderer elbowRenderer;
    public Sprite elbow1, elbow2, elbow3;

    [Header("Effects")]
    public AudioClip damageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;
    private EyeTracker eyeTracker;

    private void Start()
    {
        currentHealth = maxHealth;
        if (handRenderer == null) handRenderer = GetComponent<SpriteRenderer>();
        if (armAI == null) armAI = GetComponentInParent<BossArmAI>();
        if (uiManager == null) uiManager = FindObjectOfType<GameUIManager>();
        
        eyeTracker = FindObjectOfType<EyeTracker>();
        audioSource = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        
        UpdateVisuals();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        if (damageSound) audioSource.PlayOneShot(damageSound);
        if (eyeTracker) eyeTracker.TriggerJitter();

        UpdateVisuals();

        if (currentHealth <= 0f) Die();
    }

    private void UpdateVisuals()
    {
        if (armAI == null) return;

        float healthPercent = currentHealth / maxHealth;
        int stage = (healthPercent > 0.66f) ? 1 : (healthPercent > 0.33f) ? 2 : 3;
        bool isSlamming = (armAI.currentState == BossArmAI.State.Slam);

        // 1. Update Hand
        if (handRenderer != null)
        {
            if (isSlamming)
                handRenderer.sprite = (stage == 1) ? downSprite1 : (stage == 2) ? downSprite2 : downSprite3;
            else
                handRenderer.sprite = (stage == 1) ? upSprite1 : (stage == 2) ? upSprite2 : upSprite3;
        }

        // 2. Update Elbow (Simplfied: just changes based on health stage)
        if (elbowRenderer != null)
        {
            elbowRenderer.sprite = (stage == 1) ? elbow1 : (stage == 2) ? elbow2 : elbow3;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        foreach (AudioSource a in FindObjectsOfType<AudioSource>()) a.Stop();
        if (deathSound) audioSource.PlayOneShot(deathSound);

        if (uiManager != null) uiManager.ShowWinScreen();
    }
}