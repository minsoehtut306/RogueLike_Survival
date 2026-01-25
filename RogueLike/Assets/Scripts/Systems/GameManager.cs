using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Inspector: References
    [Header("References")]
    [Tooltip("Enemy spawner controlled by difficulty ramp.")]
    public EnemySpawner enemySpawner;

    [Tooltip("Player transform used as the reference for spawning.")]
    public Transform player;

    // Inspector: Difficulty ramp
    [Header("Difficulty Ramp (over time)")]
    [Tooltip("How many seconds until we reach max difficulty values.")]
    public float timeToMaxDifficulty = 180f;

    // Inspector: Spawner values at start
    [Header("Spawner values at start")]
    public float startSpawnInterval = 1.5f;
    public int startMaxAlive = 20;
    public float startSpawnRadius = 8f;

    // Inspector: Spawner values at max difficulty
    [Header("Spawner values at max difficulty")]
    public float endSpawnInterval = 0.35f;
    public int endMaxAlive = 80;
    public float endSpawnRadius = 10f;

    // Internal state
    private float elapsed;

    // Unity lifecycle
    private void Awake()
    {
        // Auto-find player if not assigned
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Auto-find spawner if not assigned
        if (enemySpawner == null)
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }

    private void Start()
    {
        // Apply start difficulty immediately
        ApplyDifficulty(0f);
    }

    private void Update()
    {
        if (enemySpawner == null || player == null) return;

        // Progress time
        elapsed += Time.deltaTime;

        // Convert elapsed time to a 0..1 value for interpolation
        float t = (timeToMaxDifficulty <= 0f)
            ? 1f
            : Mathf.Clamp01(elapsed / timeToMaxDifficulty);

        ApplyDifficulty(t);
    }

    // Internal helpers
    private void ApplyDifficulty(float t)
    {
        // Smoothly interpolate between start and end spawner values
        float interval = Mathf.Lerp(startSpawnInterval, endSpawnInterval, t);
        int maxAlive = Mathf.RoundToInt(Mathf.Lerp(startMaxAlive, endMaxAlive, t));
        float radius = Mathf.Lerp(startSpawnRadius, endSpawnRadius, t);

        // Push values into the spawner
        enemySpawner.SetPlayer(player);
        enemySpawner.SetDifficulty(interval, maxAlive, radius);
    }
}
