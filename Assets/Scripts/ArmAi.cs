using UnityEngine;
using System.Collections;

public class BossArmAI : MonoBehaviour
{
    public enum State { Patrol, Windup, Slam, Retract }
    public State currentState = State.Patrol;

    [Header("Detection")]
    public Transform player;
    public float detectionTimeNeeded = 3f;
    public float windupTime = 1f;
    public float detectRange = 20f;
    private float detectionTimer = 0f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float patrolWidth = 4f;
    public float slamSpeed = 30f;
    public float retractSpeed = 5f;
    public float slamDistance = 18f; // Distance down to the blades

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                // Move L to R using PingPong
                float x = Mathf.PingPong(Time.time * patrolSpeed, patrolWidth * 2) - patrolWidth;
                transform.localPosition = new Vector3(x, startLocalPos.y, 0);

                // Raycast down to find player
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectRange);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    detectionTimer += Time.deltaTime;
                    if (detectionTimer >= detectionTimeNeeded) StartCoroutine(SlamSequence());
                }
                else
                {
                    detectionTimer = 0;
                }
                break;
        }
    }

    IEnumerator SlamSequence()
    {
        // --- WINDUP ---
        currentState = State.Windup;
        Vector3 lockPos = transform.localPosition;
        float elapsed = 0;
        while (elapsed < windupTime)
        {
            // Shake effect
            transform.localPosition = lockPos + (Vector3)Random.insideUnitCircle * 0.15f;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // --- SLAM ---
        currentState = State.Slam;
        Vector3 slamTarget = new Vector3(lockPos.x, startLocalPos.y - slamDistance, 0);
        while (Vector3.Distance(transform.localPosition, slamTarget) > 0.1f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, slamTarget, slamSpeed * Time.deltaTime);
            yield return null;
        }

        // --- RETRACT ---
        currentState = State.Retract;
        yield return new WaitForSeconds(0.5f); // Pause at the bottom
        while (Vector3.Distance(transform.localPosition, new Vector3(transform.localPosition.x, startLocalPos.y, 0)) > 0.1f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(transform.localPosition.x, startLocalPos.y, 0), retractSpeed * Time.deltaTime);
            yield return null;
        }

        detectionTimer = 0;
        currentState = State.Patrol;
    }
}