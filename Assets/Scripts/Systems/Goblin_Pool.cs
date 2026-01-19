using System.Collections.Generic;
using UnityEngine;

public class GoblinPool : MonoBehaviour
{
    public GameObject goblinPrefab;
    public int initialSize = 20;

    Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateGoblin();
        }
    }

    void CreateGoblin()
    {
        GameObject g = Instantiate(goblinPrefab, transform);
        g.SetActive(false);
        pool.Enqueue(g);
    }

    public GameObject Get()
    {
        if (pool.Count == 0)
            CreateGoblin();

        GameObject g = pool.Dequeue();
        g.SetActive(true);
        return g;
    }

    public void Return(GameObject goblin)
    {
        goblin.SetActive(false);
        pool.Enqueue(goblin);
    }
}
