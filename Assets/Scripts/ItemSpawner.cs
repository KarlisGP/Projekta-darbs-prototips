using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject itemPrefab;

    public Vector2 spawnMin;
    public Vector2 spawnMax;

    [Header("Limits")]
    public int maxItems = 5;
    public float spawnCooldown = 2f;

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
                currentItems++;
            }

            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    private void SpawnItem()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(spawnMin.x, spawnMax.x),
            Random.Range(spawnMin.y, spawnMax.y)
        );

        GameObject item = Instantiate(itemPrefab, spawnPos, Quaternion.identity);

        ItemPickup pickup = item.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.SetSpawner(this);
        }
    }

    public void NotifyItemDestroyed()
    {
        currentItems = Mathf.Max(0, currentItems - 1);
    }
}