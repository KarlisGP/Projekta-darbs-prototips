using UnityEngine;
using System.Collections;

public class BossArmAI : MonoBehaviour
{
    public enum State { Patrol, Windup, Slam, Retract }
    public State currentState = State.Patrol;

    [Header("Anchors (Markers)")]
    public Transform armHome;    // Marker at top of blender
    public Transform armTarget;  // Marker at blades
    public Transform player;

    [Header("Detection Settings")]
    public float detectionWidth = 2.0f; 
    public float detectionHeight = 40.0f; 
    public float detectionTime = 3f;

    [Header("Movement Settings")]
    public float patrolSpeed = 3f;
    public float patrolWidth = 4f;
    public float slamSpeed = 50f;
    public float retractSpeed = 10f;

    private float detectionTimer;
    private float currentXOffset;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (armHome == null) return;

        if (currentState == State.Patrol)
        {
            PatrolLogic();
        }
    }

    void PatrolLogic()
    {
        // 1. SYNC UPWARD MOVEMENT: 
        // Matches the Home Marker's Y exactly so it moves with the scrolling level
        float targetY = armHome.position.y;

        // 2. PATROL LEFT/RIGHT
        currentXOffset = Mathf.PingPong(Time.time * patrolSpeed, patrolWidth * 2) - patrolWidth;
        float targetX = armHome.position.x + currentXOffset;

        // Apply position
        transform.position = new Vector3(targetX, targetY, transform.position.z);

        // 3. DETECTION: Checks if player is below the arm
        if (player != null)
        {
            float xDiff = Mathf.Abs(transform.position.x - player.position.x);
            float yDiff = transform.position.y - player.position.y;

            if (xDiff < detectionWidth && yDiff > 0 && yDiff < detectionHeight)
            {
                detectionTimer += Time.deltaTime;
                if (detectionTimer >= detectionTime) StartCoroutine(SlamSequence());
            }
            else
            {
                detectionTimer = 0;
            }
        }
    }

    IEnumerator SlamSequence()
    {
        currentState = State.Windup;
        detectionTimer = 0;

        // Shake effect
        float elapsed = 0;
        while (elapsed < 1f) 
        {
            // Stay at the Home Marker height while shaking
            transform.position = new Vector3(armHome.position.x + currentXOffset, armHome.position.y, transform.position.z) + (Vector3)Random.insideUnitCircle * 0.2f;
            elapsed += Time.deltaTime;
            yield return null;
        }

        currentState = State.Slam;
        // Move entire arm down to the Target marker
        while (transform.position.y > armTarget.position.y + 0.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, armTarget.position.y, transform.position.z), slamSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // Stay at bottom

        currentState = State.Retract;
        // Move entire arm back up to the Home marker
        while (Vector3.Distance(transform.position, new Vector3(transform.position.x, armHome.position.y, transform.position.z)) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, armHome.position.y, transform.position.z), retractSpeed * Time.deltaTime);
            yield return null;
        }

        currentState = State.Patrol;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 boxCenter = transform.position + Vector3.down * (detectionHeight / 2);
        Vector3 boxSize = new Vector3(detectionWidth * 2, detectionHeight, 1);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}