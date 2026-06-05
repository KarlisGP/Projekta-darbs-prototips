using UnityEngine;
using System.Collections;

public class BossArmAI : MonoBehaviour
{
    public enum State { Patrol, Windup, Slam, Retract }
    public State currentState = State.Patrol;

    [Header("Detection")]
    public Transform player;
    public float detectRange = 25f;
    public float detectionTimeNeeded = 3f;
    public float windupTime = 1f;
    
    [Header("Movement (Local)")]
    public float patrolSpeed = 2f;
    public float patrolWidth = 4f;
    public float slamSpeed = 30f;
    public float retractSpeed = 10f;
    public float slamLocalY = -15f; // How far DOWN from start point to hit blades

    private float detectionTimer;
    private Vector3 startLocalPos;

    void Start()
    {
        // Store the position relative to the blender
        startLocalPos = transform.localPosition;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (currentState == State.Patrol)
        {
            // 1. Patrol Left/Right
            float x = Mathf.PingPong(Time.time * patrolSpeed, patrolWidth * 2) - patrolWidth;
            transform.localPosition = new Vector3(x, startLocalPos.y, 0);

            // 2. Detection Logic
            // We shoot a raycast down to find the player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectRange);
            
            // DRAW THE RAY so you can see it in Scene View!
            Debug.DrawRay(transform.position, Vector2.down * detectRange, Color.red);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                detectionTimer += Time.deltaTime;
                if (detectionTimer >= detectionTimeNeeded) 
                {
                    StartCoroutine(SlamSequence());
                }
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
        Vector3 lockLocalPos = transform.localPosition;

        // Shake for windup
        float elapsed = 0;
        while (elapsed < windupTime)
        {
            transform.localPosition = lockLocalPos + (Vector3)Random.insideUnitCircle * 0.1f;
            elapsed += Time.deltaTime;
            yield return null;
        }

        currentState = State.Slam;
        // Slam to the local Y target (the blades)
        Vector3 targetLocalPos = new Vector3(lockLocalPos.x, slamLocalY, 0);

        while (Vector3.Distance(transform.localPosition, targetLocalPos) > 0.1f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPos, slamSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f); // Pause at bottom

        currentState = State.Retract;
        Vector3 homeLocalPos = new Vector3(transform.localPosition.x, startLocalPos.y, 0);
        while (Vector3.Distance(transform.localPosition, homeLocalPos) > 0.1f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, homeLocalPos, retractSpeed * Time.deltaTime);
            yield return null;
        }

        detectionTimer = 0;
        currentState = State.Patrol;
    }
}