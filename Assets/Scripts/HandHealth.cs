using UnityEngine;
using System.Collections.Generic;

public class HandHealth : MonoBehaviour
{
    // This helper class allows you to group 1 Renderer with its 6 specific sprites
    [System.Serializable]
    public class BossPart
    {
        public string partName; // Just for organization in Inspector
        public SpriteRenderer renderer;
        
        [Header("Up Sprites")]
        public Sprite up1;
        public Sprite up2;
        public Sprite up3;

        [Header("Down Sprites")]
        public Sprite down1;
        public Sprite down2;
        public Sprite down3;

        // Helper function to update this specific part's sprite
        public void UpdateVisual(int stage, bool isSlamming)
        {
            if (renderer == null) return;

            Sprite target = null;
            if (isSlamming)
            {
                if (stage == 0) target = down1;
                else if (stage == 1) target = down2;
                else target = down3;
            }
            else
            {
                if (stage == 0) target = up1;
                else if (stage == 1) target = up2;
                else target = up3;
            }

            if (target != null) renderer.sprite = target;
        }
    }

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

    [Header("Main Sprite (The primary one)")]
    public SpriteRenderer mainRenderer;
    public Sprite downSprite1, downSprite2, downSprite3;
    public Sprite upSprite1, upSprite2, upSprite3;

    [Header("Additional Sprites (Add the 3 others here)")]
    public List<BossPart> extraParts = new List<BossPart>();

    private EyeTracker eyeTracker;
    private AudioSource audioSource;

    private void Start()
    {
        currentHealth = maxHealth;

        if (mainRenderer == null)
            mainRenderer = GetComponent<SpriteRenderer>();

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

        if (damageSound != null) audioSource.PlayOneShot(damageSound);
        if (eyeTracker != null) eyeTracker.TriggerJitter();

        Debug.Log($"Hand HP: {currentHealth}");
        UpdateSprite();

        if (currentHealth <= 0f) Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in allAudio) a.Stop();

        if (deathSound != null) audioSource.PlayOneShot(deathSound);

        if (uiManager != null) uiManager.ShowWinScreen();
    }

    private void UpdateSprite()
    {
        if (armAI == null) return;

        float healthPercent = currentHealth / maxHealth;

        // Determine Health Stage
        int stage = (healthPercent > 0.66f) ? 0 : (healthPercent > 0.33f) ? 1 : 2;
        bool isSlamming = (armAI.currentState == BossArmAI.State.Slam);

        // 1. Update the Main Sprite
        UpdateMainSprite(stage, isSlamming);

        // 2. Update all Extra Sprites
        foreach (BossPart part in extraParts)
        {
            part.UpdateVisual(stage, isSlamming);
        }
    }

    private void UpdateMainSprite(int stage, bool isSlamming)
    {
        if (mainRenderer == null) return;

        Sprite targetSprite = null;
        if (isSlamming)
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

        if (targetSprite != null && mainRenderer.sprite != targetSprite)
        {
            mainRenderer.sprite = targetSprite;
        }
    }
}