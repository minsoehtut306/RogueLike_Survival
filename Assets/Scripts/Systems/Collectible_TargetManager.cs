using System.Collections.Generic;
using UnityEngine;

public class TargetCollectibleManager : MonoBehaviour
{
    public static TargetCollectibleManager Instance { get; private set; }

    private readonly List<TargetCollectible> collectibles = new();

    [Header("Optional filtering")]
    public string requiredId = ""; // leave empty = accept any

    // NEW: cache the last chosen target so the arrow can reference it
    private TargetCollectible currentTarget;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(TargetCollectible c)
    {
        if (c == null) return;
        if (!collectibles.Contains(c))
            collectibles.Add(c);
    }

    public void Unregister(TargetCollectible c)
    {
        if (c == null) return;

        collectibles.Remove(c);

        // NEW: if the current target is removed, clear it
        if (currentTarget == c)
            currentTarget = null;
    }

    public void OnCollected(TargetCollectible c)
    {
        // In case it’s still in the list (Destroy happens end-of-frame)
        Unregister(c);
    }

    public TargetCollectible GetClosestTarget(Vector3 fromPos)
    {
        TargetCollectible best = null;
        float bestDist = float.MaxValue;

        for (int i = collectibles.Count - 1; i >= 0; i--)
        {
            var c = collectibles[i];

            // Clean nulls (destroyed objects become null)
            if (c == null)
            {
                collectibles.RemoveAt(i);
                continue;
            }

            if (!string.IsNullOrEmpty(requiredId) && c.collectibleId != requiredId)
                continue;

            float d = (c.transform.position - fromPos).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = c;
            }
        }

        // NEW: cache chosen target
        currentTarget = best;
        return best;
    }

    public bool HasAnyTarget(Vector3 fromPos)
    {
        return GetClosestTarget(fromPos) != null;
    }

    // NEW: this is what TargetArrowWorld will call
    public Transform GetCurrentTargetTransform(Vector3 fromPos)
    {
        // Refresh selection each time (simple + always correct)
        var t = GetClosestTarget(fromPos);
        return t != null ? t.transform : null;
    }

    // Optional: if you want "last known target" without recalculating
    public Transform GetCachedTargetTransform()
    {
        return currentTarget != null ? currentTarget.transform : null;
    }
}
