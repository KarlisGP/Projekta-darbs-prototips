using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    public float smoothTime = 0.25f;
    public Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Level Boundaries")]
    public float minY;
    public float maxY;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Only follow Y (vertical)
        float targetY = target.position.y + offset.y;

        // Clamp Y within level bounds
        float clampedY = Mathf.Clamp(targetY, minY, maxY);

        // Keep current X and Z
        Vector3 finalDestination = new Vector3(
            transform.position.x, // LOCK X
            clampedY,
            transform.position.z  // keep Z (e.g. -10)
        );

        // Smooth movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            finalDestination,
            ref velocity,
            smoothTime
        );
    }
}