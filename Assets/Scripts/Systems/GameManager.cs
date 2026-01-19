using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public EnemySpawner enemySpawner;
    public Transform player;

    [Header("Difficulty Ramp (over time)")]
    [Tooltip("How many seconds until we reach max difficulty values.")]
    public float timeToMaxDifficulty = 180f; // 3 minutes

    [Header("Spawner values at start")]
    public float startSpawnInterval = 1.5f;
    public int startMaxAlive = 20;
    public float startSpawnRadius = 8f;

    [Header("Spawner values at max difficulty")]
    public float endSpawnInterval = 0.35f;
    public int endMaxAlive = 80;
    public float endSpawnRadius = 10f;

    float elapsed;

    void Awake()
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

    void Start()
    {
        ApplyDifficulty(0f);
    }

    void Update()
    {
        if (enemySpawner == null || player == null) return;

        elapsed += Time.deltaTime;

        float t = (timeToMaxDifficulty <= 0f) ? 1f : Mathf.Clamp01(elapsed / timeToMaxDifficulty);
        ApplyDifficulty(t);
    }

    void ApplyDifficulty(float t)
    {
        float interval = Mathf.Lerp(startSpawnInterval, endSpawnInterval, t);
        int maxAlive = Mathf.RoundToInt(Mathf.Lerp(startMaxAlive, endMaxAlive, t));
        float radius = Mathf.Lerp(startSpawnRadius, endSpawnRadius, t);

        enemySpawner.SetPlayer(player);
        enemySpawner.SetDifficulty(interval, maxAlive, radius);
    }
}
