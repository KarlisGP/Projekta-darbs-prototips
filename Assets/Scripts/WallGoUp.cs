using UnityEngine;

public class WallGoUp : MonoBehaviour
{
    public float speed = 4f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Vector2.up * speed;
        Debug.Log(gameObject.name + " is moving at speed: " + speed);
        rb.linearVelocity = Vector2.up * speed;
    }
    
}
