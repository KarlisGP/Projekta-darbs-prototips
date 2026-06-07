using UnityEngine;

public class BossHealthBarFollow : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform cameraTarget;

    [Header("Health Settings")]
    public HandHealth bossHealth; // your boss script
    public Transform fillTransform;

    [Header("Position Offset (screen-space feel)")]
    public Vector3 offset = new Vector3(0f, 2f, 5f);

    [Header("Scale Limits")]
    public float fullWidthScale = 1f;
    public float emptyWidthScale = 0f;

    void Start()
    {
        if (cameraTarget == null && Camera.main != null)
            cameraTarget = Camera.main.transform;
    }

    void LateUpdate()
    {
        FollowCamera();
        UpdateHealthBar();
    }

    void FollowCamera()
    {
        if (cameraTarget == null) return;

        transform.position = cameraTarget.position + offset;
    }

    void UpdateHealthBar()
    {
        if (bossHealth == null || fillTransform == null) return;

        float healthPercent = bossHealth.currentHealth / bossHealth.maxHealth;

        float scaleX = Mathf.Lerp(emptyWidthScale, fullWidthScale, healthPercent);

        Vector3 newScale = fillTransform.localScale;
        newScale.x = scaleX;

        fillTransform.localScale = newScale;
    }
}