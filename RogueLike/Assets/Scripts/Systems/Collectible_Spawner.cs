using UnityEngine;

public class FoxSpawner : MonoBehaviour
{
    // Inspector: Prefab
    [Header("Prefab")]
    [Tooltip("Collectible prefab to spawn (e.g. Fox).")]
    public GameObject foxPrefab;

    // Inspector: Spawn area
    [Header("Spawn Area (world coordinates)")]
    public Vector2 min = new(-6f, -4f);
    public Vector2 max = new(6f, 4f);

    // Inspector: Rules
    [Header("Spawn Rules")]
    [Tooltip("Delay between spawn attempts.")]
    public float spawnDelay = 2f;

    [Tooltip("Maximum number of collectibles spawned in total.")]
    public int maxTotalSpawns = 8;

    // Internal state
    private GameObject currentFox;   // only one collectible at a time
    private float timer;
    private int spawnedCount;

    // Unity lifecycle
    private void Update()
    {
        // Stop permanently once maximum total spawns reached
        if (spawnedCount >= maxTotalSpawns) return;

        // If a collectible already exists, wait until it is collected (destroyed)
        if (currentFox != null) return;

        // Count time until next spawn
        timer += Time.deltaTime;
        if (timer < spawnDelay) return;

        timer = 0f;
        SpawnFox();
    }

    // Internal helpers
    private void SpawnFox()
    {
        if (foxPrefab == null)
        {
            Debug.LogError("FoxSpawner: foxPrefab is not assigned!");
            return;
        }

        // Pick a random position within bounds
        Vector3 pos = new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            0f
        );

        currentFox = Instantiate(foxPrefab, pos, Quaternion.identity);
        spawnedCount++;
    }
}
