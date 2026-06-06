using UnityEngine;

public class HandHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Sprites")]
    public Sprite downSprite1;
    public Sprite downSprite2;
    public Sprite downSprite3;

    public Sprite upSprite1;
    public Sprite upSprite2;
    public Sprite upSprite3;

    [Header("Movement")]
    public bool movingUp;

    private SpriteRenderer spriteRenderer;
    private float previousY;

    private void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        previousY = transform.position.y;

        UpdateSprite();
    }

    private void Update()
    {
        movingUp = transform.position.y > previousY;
        previousY = transform.position.y;

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
        float healthPercent = currentHealth / maxHealth;

        int stage;

        if (healthPercent > 0.66f)
            stage = 0;
        else if (healthPercent > 0.33f)
            stage = 1;
        else
            stage = 2;

        if (!movingUp)
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
            switch (stage)
            {
                case 0: spriteRenderer.sprite = upSprite1; break;
                case 1: spriteRenderer.sprite = upSprite2; break;
                case 2: spriteRenderer.sprite = upSprite3; break;
            }
        }
    }
}