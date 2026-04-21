using UnityEngine;

public class PlayerMidAirJump : MonoBehaviour
{
    public int extraJumps = 0;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && extraJumps > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
            extraJumps--;
        }
    }

    public void EnableExtraJump()
    {
        extraJumps = 1;
    }
}