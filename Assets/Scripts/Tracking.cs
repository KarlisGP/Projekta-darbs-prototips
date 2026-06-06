using UnityEngine;
using System.Collections;

public class EyeTracker : MonoBehaviour
{
    [Header("Tracking Settings")]
    public Transform player;
    public float movementRadius = 0.2f;
    public float followSpeed = 5f;

    [Header("Blink Overlay")]
    [Tooltip("Drag the child object that has the 'Closed Eye' overlay sprite here.")]
    public GameObject blinkOverlayObject; 

    [Header("Blinking Timing")]
    public float minTimeBetweenBlinks = 2f;
    public float maxTimeBetweenBlinks = 6f;
    public float blinkDuration = 0.15f; 

    private SpriteRenderer irisRenderer;
    private Vector3 initialIrisLocalPos;

    void Start()
    {
        irisRenderer = GetComponent<SpriteRenderer>();
        initialIrisLocalPos = transform.localPosition;

        // Ensure the overlay is hidden at the start
        if (blinkOverlayObject != null) blinkOverlayObject.SetActive(false);

        // Auto-find player if missing
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

        // 1. Calculate tracking direction
        // We use the parent's position as the 'center' of the eye
        Vector3 directionToPlayer = player.position - transform.parent.position;
        
        // 2. Clamp target position to the radius
        Vector3 targetLocalPos = directionToPlayer.normalized * movementRadius;
        targetLocalPos.z = initialIrisLocalPos.z;

        // 3. Smoothly follow the player
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * followSpeed);
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks));

            // --- BLINK START ---
            if (blinkOverlayObject != null) blinkOverlayObject.SetActive(true); // Show Eyelid
            irisRenderer.enabled = false; // Hide Iris

            yield return new WaitForSeconds(blinkDuration);

            // --- BLINK END ---
            if (blinkOverlayObject != null) blinkOverlayObject.SetActive(false); // Hide Eyelid
            irisRenderer.enabled = true; // Show Iris
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.parent != null ? transform.parent.position : transform.position, movementRadius);
    }
}