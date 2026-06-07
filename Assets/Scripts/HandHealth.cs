using UnityEngine;

public class HandHealth : MonoBehaviour
{
    [Header("References")]
    public BossArmAI armAI; // Drag the object with BossArmAI here

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

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

        eyeTracker = FindObjectOfType<EyeTracker>();

        UpdateSprite();
    }

    private void Update()
    {
        UpdateSprite();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        Debug.Log($"Hand HP: {currentHealth}");

        // 🔥 Eye reaction on damage
        if (eyeTracker != null)
        {
            eyeTracker.TriggerJitter();
        }

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (armAI == null || spriteRenderer == null) return;

        float healthPercent = currentHealth / maxHealth;

        int stage;

        if (healthPercent > 0.66f)
            stage = 0;
        else if (healthPercent > 0.33f)
            stage = 1;
        else
            stage = 2;

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