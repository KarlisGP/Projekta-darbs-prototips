using UnityEngine;

public class NaturalTumble : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float minRotationSpeed = -50f;
    public float maxRotationSpeed = 50f;
    public bool randomizeStartRotation = true;

    private float currentRotationSpeed;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Set a random speed so every object spins slightly differently
        currentRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

        // Randomize the starting angle so they don't all look identical
        if (randomizeStartRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        // If it has physics, we apply the spin once as 'Torque'
        if (rb != null)
        {
            rb.angularVelocity = currentRotationSpeed;
        }
    }

    void Update()
    {
        // If the object does NOT have a Rigidbody (floating item), 
        // we rotate it manually every frame.
        if (rb == null)
        {
            transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);
        }
    }
}