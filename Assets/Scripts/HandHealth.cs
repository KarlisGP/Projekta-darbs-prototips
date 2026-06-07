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

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Automatically find the AI script if you forgot to drag it in
        if (armAI == null) armAI = GetComponentInParent<BossArmAI>();

        UpdateSprite();
    }

    private void Update()
    {
        // We update every frame to catch the moment the AI state changes
        UpdateSprite();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log($"Hand HP: {currentHealth}");
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (armAI == null) return;

        float healthPercent = currentHealth / maxHealth;
        int stage;

        // Health Stage Logic
        if (healthPercent > 0.66f) stage = 0;
        else if (healthPercent > 0.33f) stage = 1;
        else stage = 2;

        // DECISION LOGIC: Use the AI state instead of movement velocity
        // We use Down Sprites ONLY during the Slam state.
        bool useDownSprites = (armAI.currentState == BossArmAI.State.Slam);

        if (useDownSprites)
        {
            switch (stage)
            {
                case 0: spriteRenderer.sprite = downSprite1; break;
                case 1: spriteRenderer.sprite = downSprite2; break;
                case 2: spriteRenderer.sprite = downSprite3; break;
            }
        }
        else
        {
            // Use Up Sprites for Patrol, Windup, and Retract
            switch (stage)
            {
                case 0: spriteRenderer.sprite = upSprite1; break;
                case 1: spriteRenderer.sprite = upSprite2; break;
                case 2: spriteRenderer.sprite = upSprite3; break;
            }
        }
    }
}