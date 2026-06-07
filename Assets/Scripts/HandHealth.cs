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

    [Header("Hand Sprites (Combined Hand/Elbow)")]
    public SpriteRenderer handRenderer;
    public Sprite upSprite1, upSprite2, upSprite3;
    public Sprite downSprite1, downSprite2, downSprite3;

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
        if (handRenderer == null) handRenderer = GetComponent<SpriteRenderer>();
        if (armAI == null) armAI = GetComponentInParent<BossArmAI>();
        if (uiManager == null) uiManager = FindObjectOfType<GameUIManager>();
        
        eyeTracker = FindObjectOfType<EyeTracker>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        UpdateVisuals();
        UpdateHealthSprite();
    }

    // THIS IS THE MISSING PIECE: It checks for "Up/Down" state every frame
    private void Update()
    {
        if (!isDead)
        {
            UpdateVisuals();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        
        if (damageSound != null) audioSource.PlayOneShot(damageSound);
        if (eyeTracker != null) eyeTracker.TriggerJitter();

        UpdateVisuals();
        UpdateHealthSprite();

        if (currentHealth <= 0f) Die();
    }

    private void UpdateVisuals()
    {
        if (armAI == null || handRenderer == null) return;

        float healthPercent = currentHealth / maxHealth;

        // Logic for 10 HP stages
        int stage = (healthPercent > 0.66f) ? 1 : (healthPercent > 0.33f) ? 2 : 3;

        // Check if the AI is currently in the Slam state
        bool isSlamming = armAI.currentState == BossArmAI.State.Slam;

        if (isSlamming)
        {
            handRenderer.sprite = (stage == 1) ? downSprite1 : (stage == 2) ? downSprite2 : downSprite3;
        }
        else
        {
            handRenderer.sprite = (stage == 1) ? upSprite1 : (stage == 2) ? upSprite2 : upSprite3;
        }
    }

    private void UpdateHealthSprite()
    {
        if (healthSprite == null) return;
        float healthPercent = currentHealth / maxHealth;
        float scale = Mathf.Lerp(minimumSize, fullSize, healthPercent);
        healthSprite.localScale = new Vector3(scale, scale, healthSprite.localScale.z);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        foreach (AudioSource source in FindObjectsOfType<AudioSource>()) source.Stop();
        if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);

        if (uiManager != null) uiManager.ShowWinScreen();
    }
}