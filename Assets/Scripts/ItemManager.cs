using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("Items")]
    public GameObject[] itemPrefabs;

    [Header("Spawning Settings")]
    public Transform spawnPoint;
    public float spawnInterval = 1.5f;

    [Tooltip("How far left/right from the spawn point items can appear.")]
    public float horizontalRange = 5f;

    [Tooltip("How far up/down from the spawn point items can appear.")]
    public float verticalRange = 3f;

    public float spawnZ = -1f;

    [Header("Clump Prevention")]
    [Tooltip("The minimum vertical distance the blender must move before spawning another item.")]
    public float minVerticalGap = 4f;

    [Header("Size Settings")]
    public float itemScale = 2f;

    private float timer;
    private Vector3 lastSpawnPos;

    void Start()
    {
        if (spawnPoint != null)
        {
            lastSpawnPos = spawnPoint.position - new Vector3(0, minVerticalGap, 0);
        }
    }

    void Update()
    {
        if (spawnPoint == null)
            return;

        timer += Time.deltaTime;

        float distanceMoved = Mathf.Abs(spawnPoint.position.y - lastSpawnPos.y);

        if (timer >= spawnInterval && distanceMoved >= minVerticalGap)
        {
            SpawnItem();
            timer = 0f;
            lastSpawnPos = spawnPoint.position;
        }
    }

    void SpawnItem()
    {
        if (itemPrefabs.Length == 0)
            return;

        int randomIndex = Random.Range(0, itemPrefabs.Length);

        float randomX = spawnPoint.position.x +
                        Random.Range(-horizontalRange, horizontalRange);

        float randomY = spawnPoint.position.y +
                        Random.Range(-verticalRange, verticalRange);

        Vector3 spawnPos = new Vector3(
            randomX,
            randomY,
            spawnZ
        );

        GameObject newItem = Instantiate(
            itemPrefabs[randomIndex],
            spawnPos,
            Quaternion.identity
        );

        newItem.transform.localScale = new Vector3(
            itemScale,
            itemScale,
            1f
        );

        Debug.Log($"Spawned {newItem.name} at {spawnPos}");
    }

    void OnDrawGizmos()
    {
        if (spawnPoint == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            new Vector3(
                spawnPoint.position.x,
                spawnPoint.position.y,
                spawnZ
            ),
            new Vector3(
                horizontalRange * 2f,
                verticalRange * 2f,
                0.1f
            )
        );

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(
            spawnPoint.position - new Vector3(0, minVerticalGap / 2f, 0),
            new Vector3(horizontalRange * 2f, minVerticalGap, 0.1f)
        );
    }
}