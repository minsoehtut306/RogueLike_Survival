using UnityEngine;

public class FoxSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject foxPrefab;

    [Header("Spawn Area (world coords)")]
    public Vector2 min = new(-6, -4);
    public Vector2 max = new(6, 4);

    [Header("Rules")]
    public float spawnDelay = 2f;
    public int maxTotalSpawns = 8;

    private GameObject currentFox;
    private float timer;
    private int spawnedCount;

    void Update()
    {
        // If we already spawned 8 total, stop forever
        if (spawnedCount >= maxTotalSpawns) return;

        // If a fox exists, wait until it’s collected (destroyed)
        if (currentFox != null) return;

        timer += Time.deltaTime;
        if (timer < spawnDelay) return;

        timer = 0f;
        SpawnFox();
    }

    void SpawnFox()
    {
        if (foxPrefab == null)
        {
            Debug.LogError("FoxSpawner: foxPrefab is not assigned!");
            return;
        }

        Vector3 pos = new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            0f
        );

        currentFox = Instantiate(foxPrefab, pos, Quaternion.identity);
        spawnedCount++;
    }
}
