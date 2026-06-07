using UnityEngine;
using System.Collections;

public class EyeTracker : MonoBehaviour
{
    [Header("Tracking Settings")]
    public Transform player;
    public float movementRadius = 0.2f;
    public float followSpeed = 5f;

    [Header("Blink Overlay")]
    public GameObject blinkOverlayObject;

    [Header("Blinking Timing")]
    public float minTimeBetweenBlinks = 2f;
    public float maxTimeBetweenBlinks = 6f;
    public float blinkDuration = 0.15f;

    [Header("Damage Reaction (Jitter)")]
    public float jitterDuration = 0.5f;
    public float jitterStrength = 0.2f;
    public float jitterSpeed = 30f;

    private SpriteRenderer irisRenderer;
    private Vector3 initialIrisLocalPos;

    private bool isJittering = false;
    private Vector3 jitterOffset;

    void Start()
    {
        irisRenderer = GetComponent<SpriteRenderer>();
        initialIrisLocalPos = transform.localPosition;

        if (blinkOverlayObject != null)
            blinkOverlayObject.SetActive(false);

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        StartCoroutine(BlinkRoutine());
    }

    void Update()
    {
        if (player == null) return;

        if (isJittering)
        {
            DoJitter();
            return;
        }

        // Normal tracking
        Vector3 directionToPlayer = player.position - transform.parent.position;

        Vector3 targetLocalPos = directionToPlayer.normalized * movementRadius;
        targetLocalPos.z = initialIrisLocalPos.z;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetLocalPos,
            Time.deltaTime * followSpeed
        );
    }

    // 🔥 CALL THIS FROM HAND WHEN IT TAKES DAMAGE
    public void TriggerJitter()
    {
        if (!gameObject.activeInHierarchy) return;

        StopAllCoroutines();
        StartCoroutine(JitterRoutine());
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator JitterRoutine()
    {
        isJittering = true;

        float timer = 0f;

        while (timer < jitterDuration)
        {
            jitterOffset = new Vector3(
                Random.Range(-jitterStrength, jitterStrength),
                Random.Range(-jitterStrength, jitterStrength),
                0f
            );

            transform.localPosition = jitterOffset;

            timer += Time.deltaTime;
            yield return null;
        }

        isJittering = false;
        transform.localPosition = initialIrisLocalPos;
    }

    void DoJitter()
    {
        transform.localPosition += new Vector3(
            Random.Range(-jitterStrength, jitterStrength) * Time.deltaTime * jitterSpeed,
            Random.Range(-jitterStrength, jitterStrength) * Time.deltaTime * jitterSpeed,
            0f
        );
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks));

            if (blinkOverlayObject != null) blinkOverlayObject.SetActive(true);
            irisRenderer.enabled = false;

            yield return new WaitForSeconds(blinkDuration);

            if (blinkOverlayObject != null) blinkOverlayObject.SetActive(false);
            irisRenderer.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            transform.parent != null ? transform.parent.position : transform.position,
            movementRadius
        );
    }
}