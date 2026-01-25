using System.Collections.Generic;
using UnityEngine;

public class GoblinPool : MonoBehaviour
{
    // Inspector: Prefab / pool size
    [Header("Pool")]
    [Tooltip("Enemy prefab to spawn into the pool.")]
    public GameObject goblinPrefab;

    [Tooltip("How many enemies to pre-create at the start.")]
    public int initialSize = 20;

    // Internal queue storing inactive enemies
    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    // Unity lifecycle
    private void Awake()
    {
        // Pre-warm pool for performance (reduces runtime Instantiate spikes)
        for (int i = 0; i < initialSize; i++)
            CreateGoblin();
    }

    // Internal helpers
    private void CreateGoblin()
    {
        // Create a pooled enemy under this pool object
        GameObject g = Instantiate(goblinPrefab, transform);

        // Start disabled so it is not active in the scene until requested
        g.SetActive(false);

        // Store in queue
        pool.Enqueue(g);
    }

    // Public API
    public GameObject Get()
    {
        // If pool empty, expand automatically
        if (pool.Count == 0)
            CreateGoblin();

        // Activate and return the next available enemy
        GameObject g = pool.Dequeue();
        g.SetActive(true);
        return g;
    }

    public void Return(GameObject goblin)
    {
        // Return enemy to pool
        // Note: enemy scripts should call this when they "die" instead of Destroy().
        goblin.SetActive(false);
        pool.Enqueue(goblin);
    }
}
