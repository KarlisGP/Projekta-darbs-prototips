using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Patterns")]
    public GameObject[] patternPrefabs; 

    [Header("Spawning Settings")]
    public Transform spawnPoint;        
    public float spawnInterval = 1.5f;    
    public float horizontalRange = 5f;  

    [Header("Clump Prevention")]
    [Tooltip("The minimum vertical distance the blender must move before spawning another pattern.")]
    public float minVerticalGap = 4f; 

    [Header("Size Settings")]
    public float baseScale = 2f; // Fixed scale, no more randomizing

    private float timer;
    private Vector3 lastSpawnPos;

    void Start()
    {
        // Initialize lastSpawnPos so we can spawn immediately at the start
        if(spawnPoint != null) lastSpawnPos = spawnPoint.position - new Vector3(0, minVerticalGap, 0);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Check if enough time has passed AND if we have moved far enough up to avoid clumping
        float distanceMoved = Vector3.Distance(spawnPoint.position, new Vector3(spawnPoint.position.x, lastSpawnPos.y, 0));

        if (timer >= spawnInterval && distanceMoved >= minVerticalGap)
        {
            SpawnPattern();
            timer = 0;
            lastSpawnPos = spawnPoint.position; // Record where we spawned last
        }
    }

    void SpawnPattern()
    {
        if (patternPrefabs.Length == 0 || spawnPoint == null) return;

        int randomIndex = Random.Range(0, patternPrefabs.Length);
        
        float randomX = spawnPoint.position.x + Random.Range(-horizontalRange, horizontalRange);
        Vector3 spawnPos = new Vector3(randomX, spawnPoint.position.y, 0f);

        GameObject newPattern = Instantiate(patternPrefabs[randomIndex], spawnPos, Quaternion.identity, null);

        // Fixed Scale - No Randomizing
        newPattern.transform.localScale = new Vector3(baseScale, baseScale, 1f);

        Debug.Log("Spawned " + newPattern.name + " with fixed scale " + baseScale);
    }

    void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 left = new Vector3(spawnPoint.position.x - horizontalRange, spawnPoint.position.y, 0);
            Vector3 right = new Vector3(spawnPoint.position.x + horizontalRange, spawnPoint.position.y, 0);
            Gizmos.DrawLine(left, right);
            
            // Visualize the 'Gap' zone to prevent clumping
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(spawnPoint.position - new Vector3(0, minVerticalGap/2, 0), new Vector3(horizontalRange * 2, minVerticalGap, 0.1f));
        }
    }
}