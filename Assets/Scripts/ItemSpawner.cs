using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject itemPrefab;

    [Header("Hand Target")]
    public HandHealth targetHand;

    [Header("Spawn Area")]
    public Vector2 spawnMin = new Vector2(-5f, -2f);
    public Vector2 spawnMax = new Vector2(5f, 2f);

    [Header("Spawn Settings")]
    public float spawnCooldown = 2f;
    public int maxItems = 5;

    private int currentItems = 0;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentItems < maxItems)
            {
                SpawnItem();
            }

            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    private void SpawnItem()
    {
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnMin.x, spawnMax.x),
            Random.Range(spawnMin.y, spawnMax.y)
        );

        GameObject item = Instantiate(
            itemPrefab,
            spawnPosition,
            Quaternion.identity
        );

        currentItems++;

        ItemPickup pickup = item.GetComponent<ItemPickup>();

        if (pickup != null)
        {
            pickup.SetSpawner(this);
            pickup.targetHand = targetHand;
        }
    }

    public void NotifyItemDestroyed()
    {
        currentItems = Mathf.Max(0, currentItems - 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 center = new Vector3(
            (spawnMin.x + spawnMax.x) * 0.5f,
            (spawnMin.y + spawnMax.y) * 0.5f,
            0f
        );

        Vector3 size = new Vector3(
            Mathf.Abs(spawnMax.x - spawnMin.x),
            Mathf.Abs(spawnMax.y - spawnMin.y),
            0f
        );

        Gizmos.DrawWireCube(center, size);
    }
}