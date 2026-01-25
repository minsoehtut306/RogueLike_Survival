using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TargetCollectible : MonoBehaviour
{
    // Inspector: Identification
    [Tooltip("Optional identifier used for filtering (e.g. 'Fox').")]
    public string collectibleId = "Fox";

    // Unity lifecycle
    private void Reset()
    {
        // Ensure collider is set as trigger for pickup detection
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        // Register with the manager so arrows/UI can target this collectible
        var mgr = TargetCollectibleManager.Instance;
        if (mgr != null)
            mgr.Register(this);
    }

    private void OnDisable()
    {
        // Unregister when disabled or destroyed
        var mgr = TargetCollectibleManager.Instance;
        if (mgr != null)
            mgr.Unregister(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the player
        if (!other.CompareTag("Player")) return;

        // Grant weapon to the player
        var weaponManager = other.GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.GiveNextWeapon();
        }
        else
        {
            Debug.LogWarning("TargetCollectible: Player has no WeaponManager.");
        }

        // Notify manager so targeting updates immediately
        var mgr = TargetCollectibleManager.Instance;
        if (mgr != null)
            mgr.OnCollected(this);

        // Remove collectible from the world
        Destroy(gameObject);
    }
}
