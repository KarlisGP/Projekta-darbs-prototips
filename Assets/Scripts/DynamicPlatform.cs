using UnityEngine;
using System.Collections;

public class DynamicPlatform : MonoBehaviour
{
    public float switchInterval = 2f;
    public float respawnDelay = 1.5f;

    private bool isJumpMode = true;

    private Collider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        StartCoroutine(SwitchMode());
        UpdateVisual();
    }

    IEnumerator SwitchMode()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchInterval);
            isJumpMode = !isJumpMode;
            UpdateVisual();
        }
    }

    void UpdateVisual()
    {
        if (sr != null)
            sr.color = isJumpMode ? Color.green : Color.red;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (isJumpMode)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.GiveExtraJump(1);
            }
        }
        else
        {
            StartCoroutine(DisablePlatform());
        }
    }

    IEnumerator DisablePlatform()
    {
        col.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        col.enabled = true;
    }
}