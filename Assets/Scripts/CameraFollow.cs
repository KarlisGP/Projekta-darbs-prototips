using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Drag your Player object here

    [Header("Settings")]
    public float smoothTime = 0.25f; // How "heavy" the camera feels
    public Vector3 offset = new Vector3(0, 0, -10f); // Keep Z at -10 for 2D

    [Header("Level Boundaries")]
    // Adjust these in the inspector to stop the camera at level edges
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Determine where the camera wants to be
        Vector3 targetPosition = target.position + offset;

        // 2. Clamp that position so it stays inside your level boundaries
        float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);

        Vector3 finalDestination = new Vector3(clampedX, clampedY, targetPosition.z);

        // 3. Smoothly move to that destination
        transform.position = Vector3.SmoothDamp(transform.position, finalDestination, ref velocity, smoothTime);
    }
}