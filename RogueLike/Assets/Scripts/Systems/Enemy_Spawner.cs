using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Inspector: Pool / prefab source
    [Header("References")]
    [Tooltip("Pool that provides enemy instances.")]
    public GoblinPool pool;

    // Inspector: Spawn settings
    [Header("Spawn Settings")]
    [Tooltip("Time between spawn attempts (seconds).")]
    public float spawnInterval = 1.5f;

    [Tooltip("Distance from the player where enemies spawn.")]
    public float spawnRadius = 3f;

    [Tooltip("Maximum number of enemies allowed alive at once.")]
    public int maxAlive = 30;

    // Internal state
    private Transform player;
    private float timer;

    // Unity lifecycle
    private void Start()
    {
        // Auto-find player if not assigned via GameManager
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        // Safety: do nothing if missing references
        if (player == null || pool == null) return;

        // Do not spawn if we already have enough enemies
        if (CountAlive() >= maxAlive) return;

        // Time-based spawning
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    // Spawning
    private void Spawn()
    {
        // Pick a random direction around the player
        Vector2 offset = Random.insideUnitCircle;

        // Very small vector can cause "no movement" issues, so force a direction
        if (offset.sqrMagnitude < 0.001f)
            offset = Vector2.right;

        // Normalise so spawn distance is consistent
        offset = offset.normalized * spawnRadius;

        // Convert to world position around player
        Vector3 pos = player.position + new Vector3(offset.x, offset.y, 0f);

        // Get an enemy from the pool and place it
        GameObject goblin = pool.Get();
        goblin.transform.position = pos;
    }

    // Internal helpers
    private int CountAlive()
    {
        // Simple (but expensive) method: counts objects tagged "Enemy".
        // If performance becomes an issue, switch to tracking alive count in the pool.
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    // Public API (called by GameManager)
    public void SetPlayer(Transform p)
    {
        player = p;
    }

    public void SetDifficulty(float interval, int maxAliveCount, float radius)
    {
        // Called every frame by GameManager difficulty ramp
        spawnInterval = interval;
        maxAlive = maxAliveCount;
        spawnRadius = radius;
    }
}
