using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GoblinPool pool;
    public float spawnInterval = 1.5f;
    public float spawnRadius = 3f;
    public int maxAlive = 30;

    Transform player;
    float timer;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || pool == null) return;

        if (CountAlive() >= maxAlive) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        Vector2 offset = Random.insideUnitCircle;
        if (offset.sqrMagnitude < 0.001f)
            offset = Vector2.right;

        offset = offset.normalized * spawnRadius;

        Vector3 pos = player.position + new Vector3(offset.x, offset.y, 0f);

        GameObject goblin = pool.Get();
        goblin.transform.position = pos;
    }

    int CountAlive()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public void SetPlayer(Transform p)
    {
        player = p;
    }

    public void SetDifficulty(float interval, int maxAliveCount, float radius)
    {
        spawnInterval = interval;
        maxAlive = maxAliveCount;
        spawnRadius = radius;
    }

}
