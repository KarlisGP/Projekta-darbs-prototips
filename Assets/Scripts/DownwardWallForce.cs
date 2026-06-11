using UnityEngine;

public class DownwardWallForce : MonoBehaviour
{
    public float downwardForce = 15f;
    public float maxFallSpeed = 8f;
    public float pushAwayForce = 5f;

    private Rigidbody2D playerRb;
    private bool touchingWall;
    private Vector2 wallNormal;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerRb = collision.rigidbody;
        touchingWall = true;

        wallNormal = Vector2.zero;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                wallNormal = contact.normal;
                break;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerRb = collision.rigidbody;
        touchingWall = true;

        // constantly refresh normal (important for edge colliders)
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                wallNormal = contact.normal;
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        touchingWall = false;
        playerRb = null;
        wallNormal = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (!touchingWall || playerRb == null)
            return;

        Vector2 vel = playerRb.linearVelocity;

        // cancel upward movement
        if (vel.y > 0)
            vel.y = 0;

        // downward push
        vel.y -= downwardForce * Time.fixedDeltaTime;

        if (vel.y < -maxFallSpeed)
            vel.y = -maxFallSpeed;

        // REAL push away from wall using normal
        vel += wallNormal * pushAwayForce * Time.fixedDeltaTime;

        playerRb.linearVelocity = vel;
    }
}