using System.Collections.Generic;
using UnityEngine;

public class TargetCollectibleManager : MonoBehaviour
{
    // Singleton access
    public static TargetCollectibleManager Instance { get; private set; }

    // Internal list of active collectibles
    private readonly List<TargetCollectible> collectibles = new();

    // Inspector: Optional filtering
    [Header("Optional Filtering")]
    [Tooltip("If set, only collectibles with this ID will be considered.")]
    public string requiredId = "";

    // Cached target (optional optimisation / convenience)
    private TargetCollectible currentTarget;

    // Unity lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Public API
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

        // Clear cached target if it was removed
        if (currentTarget == c)
            currentTarget = null;
    }

    public void OnCollected(TargetCollectible c)
    {
        // Ensure removal even if Destroy happens end-of-frame
        Unregister(c);
    }

    public TargetCollectible GetClosestTarget(Vector3 fromPos)
    {
        TargetCollectible best = null;
        float bestDist = float.MaxValue;

        for (int i = collectibles.Count - 1; i >= 0; i--)
        {
            var c = collectibles[i];

            // Clean up destroyed entries
            if (c == null)
            {
                collectibles.RemoveAt(i);
                continue;
            }

            // Optional ID filtering
            if (!string.IsNullOrEmpty(requiredId) && c.collectibleId != requiredId)
                continue;

            float d = (c.transform.position - fromPos).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = c;
            }
        }

        currentTarget = best;
        return best;
    }

    public Transform GetCurrentTargetTransform(Vector3 fromPos)
    {
        var t = GetClosestTarget(fromPos);
        return t != null ? t.transform : null;
    }

    public Transform GetCachedTargetTransform()
    {
        return currentTarget != null ? currentTarget.transform : null;
    }
}
