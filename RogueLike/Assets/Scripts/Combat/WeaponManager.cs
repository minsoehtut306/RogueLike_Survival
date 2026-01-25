using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // Inspector: Weapon prefabs
    [Header("Weapon Prefabs")]
    [Tooltip("Prefab for the fire arrow weapon (must include ArrowWeapon component).")]
    public GameObject fireArrowPrefab;

    [Tooltip("Prefab for the water arrow weapon (must include ArrowWeapon component).")]
    public GameObject waterArrowPrefab;

    // Inspector: Limits
    [Header("Limits")]
    [Tooltip("Maximum number of weapons that can orbit the player.")]
    public int maxWeapons = 8;

    // Inspector: Orbit layout
    [Header("Orbit Layout")]
    [Tooltip("Distance from the player for orbiting weapons.")]
    public float weaponRadius = 0.75f;

    [Tooltip("Local offset from the player pivot (useful if player sprite pivot is not centered).")]
    public Vector2 orbitCenterOffset = new Vector2(0f, -0.15f);

    [Tooltip("Scales Y axis for an ellipse (1 = circle).")]
    public float yEllipseScale = 1.0f;

    // Inspector: Orbit arc
    [Header("Orbit Arc (no weapons below player)")]
    [Tooltip("Start angle in degrees (0 = right, 90 = up).")]
    public float arcStartDeg = 30f;

    [Tooltip("End angle in degrees (0 = right, 90 = up).")]
    public float arcEndDeg = 330f;

    // Internal state
    private readonly List<GameObject> weapons = new();
    private int pickupIndex = 0;

    // Public API
    public void GiveNextWeapon()
    {
        // Do not exceed limit
        if (weapons.Count >= maxWeapons) return;

        // Choose prefab based on pickup order (alternating pattern)
        GameObject prefab = PickNextPrefab();
        if (prefab == null) return;

        // Spawn weapon as a child so it naturally orbits with the player
        GameObject weapon = Instantiate(prefab, transform);
        weapons.Add(weapon);

        // Assign player reference so ArrowWeapon knows who it belongs to
        ArrowWeapon arrowWeapon = weapon.GetComponent<ArrowWeapon>();
        if (arrowWeapon != null)
        {
            arrowWeapon.player = transform;
        }

        // Update positions of all weapons
        RepositionWeapons();
    }

    // Internal helpers
    private GameObject PickNextPrefab()
    {
        // Alternate fire/water for now.
        // Later you can replace this with a proper drop table or shop system.
        GameObject prefab = (pickupIndex % 2 == 0) ? fireArrowPrefab : waterArrowPrefab;
        pickupIndex++;

        return prefab;
    }

    private void RepositionWeapons()
    {
        int count = weapons.Count;
        if (count == 0) return;

        // Compute arc range properly even if end < start (wrap around)
        float arcRange = arcEndDeg - arcStartDeg;
        if (arcRange <= 0f) arcRange += 360f;

        // If only 1 weapon, place it in the middle of the arc.
        // Otherwise spread evenly across the arc.
        float step = (count == 1) ? 0f : arcRange / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angleDeg = (count == 1)
                ? arcStartDeg + arcRange * 0.5f
                : arcStartDeg + step * i;

            float rad = angleDeg * Mathf.Deg2Rad;

            // Ellipse orbit: x uses cos, y uses sin with optional scaling
            float x = Mathf.Cos(rad) * weaponRadius;
            float y = Mathf.Sin(rad) * weaponRadius * yEllipseScale;

            // Offset shifts orbit center relative to player
            Vector3 localPos = new Vector3(
                orbitCenterOffset.x + x,
                orbitCenterOffset.y + y,
                0f
            );

            // Keep weapon as child and update its local position
            weapons[i].transform.localPosition = localPos;
        }
    }
}
