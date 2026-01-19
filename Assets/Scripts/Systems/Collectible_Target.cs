using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TargetCollectible : MonoBehaviour
{
    [Tooltip("Used only for filtering (optional). Example: Fox")]
    public string collectibleId = "Fox";

    private void Reset()
    {
        // Make sure collider is trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        var mgr = TargetCollectibleManager.Instance;
        if (mgr != null) mgr.Register(this);
    }

    private void OnDisable()
    {
        var mgr = TargetCollectibleManager.Instance;
        if (mgr != null) mgr.Unregister(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Give weapon
        var weaponManager = other.GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.GiveNextWeapon();
        }
        else
        {
            Debug.LogWarning("Player has no WeaponManager component.");
        }

        // Notify manager (so arrow updates immediately)
        var mgr = TargetCollectibleManager.Instance;
        if (mgr != null) mgr.OnCollected(this);

        // Destroy collectible
        Destroy(gameObject);
    }
}
