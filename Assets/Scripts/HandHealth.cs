using UnityEngine;

public class HandHealth : MonoBehaviour
{
    [Header("References")]
    public BossArmAI armAI;
    public GameUIManager uiManager; // Add this reference in the Inspector

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    private bool isDead = false; // Prevents triggering win screen multiple times

    [Header("Sprites (Damaged versions)")]
    public Sprite downSprite1;
    public Sprite downSprite2;
    public Sprite downSprite3;
    public Sprite upSprite1;
    public Sprite upSprite2;
    public Sprite upSprite3;

    private SpriteRenderer spriteRenderer;
    private EyeTracker eyeTracker;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (armAI == null)
            armAI = GetComponentInParent<BossArmAI>();

        // Automatically try to find the UI Manager if not assigned
        if (uiManager == null)
            uiManager = FindObjectOfType<GameUIManager>();

        eyeTracker = FindObjectOfType<EyeTracker>();

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

        Debug.Log($"Hand HP: {currentHealth}");

        if (eyeTracker != null)
            eyeTracker.TriggerJitter();

        UpdateSprite();

        // CHECK FOR WIN CONDITION
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        if (uiManager != null)
        {
            uiManager.ShowWinScreen();
        }
        else
        {
            Debug.LogError("Win Screen triggered but GameUIManager is missing from the scene!");
        }
    }

    private void UpdateSprite()
    {
        if (armAI == null || spriteRenderer == null) return;

        float healthPercent = currentHealth / maxHealth;
        int stage = (healthPercent > 0.66f) ? 0 : (healthPercent > 0.33f ? 1 : 2);
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
            spriteRenderer.sprite = targetSprite;
    }
}